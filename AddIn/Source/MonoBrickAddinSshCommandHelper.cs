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
using MonoDevelop.Core.Execution;
using Renci.SshNet;

namespace MonoBrickAddin
{
	class SshCommandHelper
	{
		private SshClient _sshClient = null;
		private IConsole _console = null;
		private bool _verbose = false;
		private ManualResetEvent _executed;

		public SshCommandHelper(string IPAddress, ManualResetEvent executed = null, IConsole console = null, bool verbose = false)
		{
			_executed = executed;
			_console = console;
			_verbose = verbose;
			_sshClient = new SshClient(IPAddress, "root", "");
		}

		public void Cancel()
		{
			if (_sshClient.IsConnected)
				_sshClient.Disconnect();
		}

		public void WriteSSHCommand(string command, bool waitUntilFinished = false)
		{
			if (_executed != null)
				_executed.Reset();

			_sshClient.Connect();

			using (var stream = _sshClient.CreateShellStream("MonoBrickShell", 80, 24, 800, 600, 1024))
			{
				var writer = new StreamWriter(stream);
				writer.AutoFlush = true;

				WaitForPrompt(stream, true);

				writer.WriteLine(command);

				if (_executed != null)
					_executed.Set();

				WaitForPrompt(stream, !waitUntilFinished);
			}

			_sshClient.Disconnect();
		}

		private void WaitForPrompt(ShellStream stream, bool timeOut)
		{
			if (_verbose)
			{
				if (timeOut)
				{
					stream.Expect(TimeSpan.FromSeconds(5), new Renci.SshNet.ExpectAction("#",
						(output) =>
					{
						if (_console != null)
							_console.Log.Write(output);
					}));
				}
				else
				{
					stream.Expect(new Renci.SshNet.ExpectAction("#",
						(output) =>
					{
						if (_console != null)
							_console.Log.Write(output);
					}));
				}
			}
			else
			{
				if (timeOut)
					stream.Expect("#", TimeSpan.FromSeconds(5));
				else
					stream.Expect("#");
			}
		}
	}
}