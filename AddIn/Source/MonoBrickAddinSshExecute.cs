// MonoBrickAddinSshExecute.cs
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
using System.Text;
using System.IO;
using System.Threading;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using Renci.SshNet;
using System.Text.RegularExpressions;

namespace MonoBrickAddin
{
	class SshExecute : IProcessAsyncOperation
	{
		private string _sdbOptions;
		private SshCommandHelper _sshHelper = null;
		private IConsole _console = null;
		private MonoBrickExecutionCommand _cmd;
		private ManualResetEvent _executed = new ManualResetEvent(false);

		// IProcessAsyncOperation
		public int ExitCode { get; private set; }

		public int ProcessId { get; private set; }

		void IDisposable.Dispose()
		{
			_sshHelper.Cancel();
		}

		// IAsyncOperation
		public bool IsCompleted { get; private set; }

		public bool Success { get; private set; }

		public bool SuccessWithWarnings { get; private set; }

		public event OperationHandler Completed;

		private ManualResetEvent wait = new ManualResetEvent(false);

		public void WaitForCompleted()
		{
			WaitHandle.WaitAll(new WaitHandle [] { wait });
		}

		public void WaitForExecuted()
		{
			WaitHandle.WaitAll(new WaitHandle [] { _executed });
		}

		public void Cancel()
		{
			_sshHelper.Cancel();

			//DoKill ();
			wait.Set();
		}

		public SshExecute(string IPAddress, MonoBrickExecutionCommand cmd, string sdbOptions, IConsole console, bool verbose)
		{
			_cmd = cmd;
			_sdbOptions = sdbOptions;
			_console = console;
			_sshHelper = new SshCommandHelper(IPAddress, _executed, console, verbose);
			Success = false;
			SuccessWithWarnings = false;
		}

		public void Execute()
		{
			Success = false;
			SuccessWithWarnings = false;

			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					DoRun();
					Success = true;
				}
				catch (Exception ex)
				{
					Success = false;
					_console.Log.WriteLine(ex.Message);
				}
				finally
				{
					try
					{
						_sshHelper.Cancel();
					}
					catch (Exception ex)
					{
						_console.Log.WriteLine(ex.Message);
						Success = false;
					}
				}
				IsCompleted = true;
				wait.Set();
				if (Completed != null)
					Completed(this);

				_cmd.LastError = _sshHelper.ErrorCode;
			});
		}

		private void DoKill(string appToKill)
		{
			_sshHelper.WriteSSHCommand(GetKillCommand(appToKill));
		}

		private void DoRun()
		{
			_sshHelper.WriteSSHCommand(GetRunString(), true);
		}

		private string GetRunString()
		{
			Regex regexAot = new Regex("(-{1,2}(full)?-{0,2}aot=?(full)?)", RegexOptions.Compiled);

			string additionalParameters = string.IsNullOrEmpty(_cmd.Config.CommandLineParameters)
				? "" : _cmd.Config.CommandLineParameters;

			MatchCollection matches = regexAot.Matches(additionalParameters);

			bool aotMode = matches.Count > 0;

			if (!aotMode && _cmd.AOT)
			{
				aotMode = true;
				additionalParameters += "--aot=full";
			}

			bool debugging = !string.IsNullOrEmpty(_sdbOptions);
			bool debugMode = false;

			var sb = new StringBuilder();

			sb.Append("mono ");

			if (_cmd.Config.ToString().Contains("Debug"))
			{
				sb.Append("--debug ");
				debugMode = true;
			}

			if (aotMode && (debugging || debugMode))
			{
				aotMode = false;
				_console.Log.WriteLine("Aot not supported in debug mode or for softdebugger, discarding!");
				additionalParameters = regexAot.Replace(additionalParameters, "");
			}

			if (!string.IsNullOrEmpty(_sdbOptions))
				sb.AppendFormat("--debugger-agent={0}", _sdbOptions);

			sb.AppendFormat("{0} '{1}'", additionalParameters, _cmd.DeviceExePath);

			if (aotMode)
				sb.AppendFormat(" && mono '{0}'", _cmd.DeviceExePath);

			return sb.ToString();
		}

		private string GetKillCommand(string appToKill)
		{
			if (appToKill == "")
				appToKill = _cmd.AppName;

			string killString = string.Format("kill -9 `ps | grep {0} | grep mono | awk '{{print $1}}'`", appToKill);
			return killString;
		}
	}
}