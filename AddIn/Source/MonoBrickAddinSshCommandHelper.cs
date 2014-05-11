// MonoBrickAddinSshCommandHelper.cs
//
// Author:
//       Bernhard Straub
// 
// Copyright (c) 2014 Bernhard Straub
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.

using System;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Globalization;
using MonoDevelop.Core.Execution;
using Renci.SshNet;

namespace MonoBrickAddin
{
	class SshCommandHelper
	{
		public int ErrorCode { get; private set; }

		private SshClient _sshClient = null;
		private IConsole _console = null;
		private bool _verbose = false;
		private ManualResetEvent _executed;

		// regex
		public Regex RegexAot = new Regex("(-{1,2}(full)?-{0,2}aot=?(full)?)", RegexOptions.Compiled);
		public Regex RegexExeDll = new Regex(@"\b\w+\.(exe|dll)(?!\.)\b", RegexOptions.Compiled);
		public Regex RegexExecutionFiles = new Regex(@"\b\w+\.(exe|dll)(?:\.mdb|\.pdb)?(?!\.so)\b", RegexOptions.Compiled);
		public Regex RegexSo = new Regex(@"\b\w+\.(exe|dll)(?=\.so)\b", RegexOptions.Compiled);
		public Regex RegexResponse = new Regex(@"(?<=sh: )(-?[0-9]+)(?!"")", RegexOptions.Compiled);
		public Regex RegexExtension = new Regex(@"$(?<=\.(exe|mdb|pdb|dll))", RegexOptions.Compiled);

		public SshCommandHelper(string IPAddress, ManualResetEvent executed = null, IConsole console = null, bool verbose = false)
		{
			_executed = executed;
			_console = console;
			_verbose = verbose;
			_sshClient = new SshClient(IPAddress, "root", "");
			ErrorCode = 0;
		}

		public void Cancel()
		{
			if (_sshClient.IsConnected)
				_sshClient.Disconnect();
		}

		public string WriteSSHCommand(string command, bool waitUntilFinished = false, bool scriptWithEcho = false)
		{
			string result = "";

			if (_executed != null)
				_executed.Reset();

			_sshClient.Connect();

			using (var stream = _sshClient.CreateShellStream("MonoBrickShell", 200, 80, 800, 600, 1024))
			{
				ErrorCode = -1;
				var writer = new StreamWriter(stream);
				writer.AutoFlush = true;

				WaitForPrompt(stream, true);

				writer.WriteLine(command);

				if (_executed != null)
					_executed.Set();

				result = WaitForPrompt(stream, !waitUntilFinished);

				// get error code
				if (!scriptWithEcho)
				{
					writer.WriteLine("$?");
					result = WaitForPrompt(stream, true);

					string error = RegexResponse.Match(result).Value;

					if (!String.IsNullOrEmpty(error))
						ErrorCode = int.Parse(error);
				}
			}

			_sshClient.Disconnect();

			if (_verbose && _console != null)
				_console.Log.Write(Environment.NewLine);

			return result;
		}

		private string WaitForPrompt(ShellStream stream, bool timeOut)
		{
			string result = "";

			if (timeOut)
			{
				stream.Expect(TimeSpan.FromSeconds(5), new Renci.SshNet.ExpectAction("#",
					(output) =>
					{
						result = output;
					}));
			}
			else
			{
				stream.Expect(new Renci.SshNet.ExpectAction("#",
					(output) =>
					{
						result = output;
					}));
			}

			if (_verbose && _console != null)
				_console.Log.Write(result);

			return result;
		}

		public string GetKillCommand(string appToKill)
		{
			string killString = string.Format("kill -9 `ps | grep {0} | grep mono | awk '{{print $1}}'`", appToKill);
			return killString;
		}

		public string GetFileListCommand(string remotePath)
		{
			string getFileListString = string.Format(@"find {0} -maxdepth 1 -type f -regex "".*/.*\.\(exe\|mdb\|pdb\|dll\|so\)""", remotePath);
			return getFileListString;
		}

		public string GetDirectoryCommand(string remotePath)
		{
			string makeDirString = string.Format("mkdir -p {0}", remotePath);
			return makeDirString;
		}

		public string GetCleanCommand(string remotePath, bool bFull)
		{
			string cleanString = "";

			if (bFull)
				cleanString = string.Format(@"find {0}  -maxdepth 1 -type f -regex "".*/.*\.\(exe\|mdb\|pdb\|so\)"" -delete", remotePath);
			else	
				cleanString = string.Format(@"find {0}  -maxdepth 1 -type f -regex "".*/.*\.\(exe\|mdb\|pdb\|exe.so\)"" -delete", remotePath);

			return cleanString;
		}
	}
}