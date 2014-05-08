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
		static private Regex regexResponse = new Regex(@"(?<=sh: )(-?[0-9]+)(?!"")", RegexOptions.Compiled);

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

		public void WriteSSHCommand(string command, bool waitUntilFinished = false, bool scriptWithEcho = false)
		{
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

				string result = WaitForPrompt(stream, !waitUntilFinished);

				// get error code
				if (!scriptWithEcho)
				{
					writer.WriteLine("$?");
					result = WaitForPrompt(stream, true);
				}
				string error = regexResponse.Match(result).Value;

				if (!String.IsNullOrEmpty(error))
					ErrorCode = int.Parse(error);
			}

			_sshClient.Disconnect();

			if (_verbose && _console != null)
				_console.Log.Write(Environment.NewLine);
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
	}
}