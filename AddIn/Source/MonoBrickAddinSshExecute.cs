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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using Renci.SshNet;

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
			wait.Set();
		}

		public SshExecute(string IPAddress, MonoBrickExecutionCommand cmd, string sdbOptions, bool verbose)
		{
			_cmd = cmd;
			_sdbOptions = sdbOptions;
			_console = cmd.Console;
			_sshHelper = new SshCommandHelper(IPAddress, _executed, cmd.Console, verbose);
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
			_sshHelper.WriteSSHCommand(_sshHelper.GetKillCommand(appToKill));
		}

		private void DoRun()
		{
			_sshHelper.WriteSSHCommand(GetRunString(), true);
		}

		private string GetRunString()
		{
			string additionalParameters = string.IsNullOrEmpty(_cmd.Config.CommandLineParameters)
				? "" : _cmd.Config.CommandLineParameters;

			MatchCollection matches = _sshHelper.RegexAot.Matches(additionalParameters);

			if (matches.Count > 0)
			{
				_console.Log.WriteLine("Please use build in AOT run mode!");
				additionalParameters = _sshHelper.RegexAot.Replace(additionalParameters, "");
			}

			if (_cmd.AOT)
				DoAOTCompile();

			var sb = new StringBuilder();

			sb.Append("mono ");

			if (_cmd.Config.ToString().Contains("Debug"))
				sb.Append("--debug ");

			if (!string.IsNullOrEmpty(_sdbOptions))
				sb.AppendFormat("--debugger-agent={0}", _sdbOptions);

			if (_cmd.AOT)
				sb.Append("--full-aot ");

			sb.AppendFormat("{0} '{1}'", additionalParameters, _cmd.DeviceExePath);

			return sb.ToString();
		}

		private void DoAOTCompile()
		{
			string fileList = _sshHelper.WriteSSHCommand(_sshHelper.GetFileListCommand(_cmd.DeviceDirectory), false, true);

			var exeDllList = _sshHelper.RegexExeDll.Matches(fileList)
				.OfType<Match>()
				.Select(m => m.Groups[0].Value)
				.ToArray();

			var soList = _sshHelper.RegexSo.Matches(fileList)
				.OfType<Match>()
				.Select(m => m.Groups[0].Value)
				.ToArray();

			var compileList = exeDllList.Where(fileNative => !soList.Contains(fileNative)).ToArray();

			if (compileList.Count() == 0)
				_console.Log.WriteLine("All files compiled, nothing to do!");

			foreach (var file in compileList)
			{
				string compileTarget = string.Format(@"mono --aot=full {0}{1}", _cmd.DeviceDirectory, file);
				_console.Log.WriteLine(string.Format("Compiling {0} for AOT execution", file));
				_sshHelper.WriteSSHCommand(compileTarget, true);
			}
		}
			

	}
}