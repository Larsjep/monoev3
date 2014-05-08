// MonoBrickAddinDebuggerSession.cs
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
using System.Threading;
using Mono.Debugging.Client;
using Mono.Debugging.Soft;
using MonoDevelop.Core;

namespace MonoBrickAddin
{
	public class MonoBrickSoftDebuggerSession : Mono.Debugging.Soft.SoftDebuggerSession
	{
		SshExecute process;
		/*protected override string GetConnectingMessage (SoftDebuggerStartInfo dsi)
		{
			return string.Format ("Waiting for debugger to connect on {0}:{1}...", dsi.Address, dsi.DebugPort);
		}*/
		protected override void OnRun(DebuggerStartInfo startInfo)
		{
			var dsi = (MonoBrickSoftDebuggerStartInfo)startInfo;
			StartProcess(dsi);
			Thread.Sleep(500);
			StartConnecting(dsi);
		}

		protected override void EndSession()
		{
			base.EndSession();
			EndProcess();
		}

		void StartProcess(MonoBrickSoftDebuggerStartInfo dsi)
		{
			SoftDebuggerListenArgs args = (SoftDebuggerListenArgs)dsi.StartArgs;

			string EV3IPAddress = UserSettings.Instance.IPAddress;
			string debugOptions = string.Format("transport=dt_socket,address=0.0.0.0:{0},server=y", args.DebugPort);
			bool EV3Verbose = UserSettings.Instance.Verbose;

			process = MonoBrickUtility.ExecuteCommand(EV3IPAddress, dsi.ExecutionCommand, debugOptions, EV3Verbose);
			process.Execute();
			process.WaitForExecuted();
		}

		void EndProcess()
		{
			if (process == null)
				return;
			if (!process.IsCompleted)
			{
				try
				{
					process.Cancel();
				}
				catch
				{
				}
			}
		}

		protected override void OnExit()
		{
			base.OnExit();
			EndProcess();
		}
	}
}

