// MonoBrickAddinDebuggerEngine.cs
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
using System.Net;
using MonoDevelop.Debugger;
using Mono.Debugging.Client;
using MonoDevelop.Core.Execution;
using Mono.Debugging.Soft;
using MonoDevelop.Core;
using MonoDevelop.Debugger.Soft;

namespace MonoBrickAddin
{
	public class MonoBrickSoftDebuggerEngine: IDebuggerEngine
	{
		public string Id { get { return "MonoBrickAddin.SoftDebugger"; } }

		public string Name { get { return "Mono Soft Debugger for MonoBrick"; } }

		public bool CanDebugCommand(ExecutionCommand command)
		{
			return command is MonoBrickExecutionCommand;
		}

		public DebuggerStartInfo CreateDebuggerStartInfo(ExecutionCommand command)
		{
			var cmd = (MonoBrickExecutionCommand)command;
			string EV3IPAddress = UserSettings.Instance.IPAddress;
			string EV3DebuggerPort = UserSettings.Instance.DebugPort;

			var debuggerAddress = IPAddress.Parse(EV3IPAddress);
			int debuggerPort = int.Parse(EV3DebuggerPort);

			var startInfo = new MonoBrickSoftDebuggerStartInfo(debuggerAddress, debuggerPort, cmd);
			SoftDebuggerEngine.SetUserAssemblyNames(startInfo, cmd.UserAssemblyPaths);
			return startInfo;
		}

		public DebuggerSession CreateSession()
		{
			return new MonoBrickSoftDebuggerSession();
		}

		public ProcessInfo[] GetAttachableProcesses()
		{
			return new ProcessInfo[0];
		}
	}

	class MonoBrickSoftDebuggerStartInfo : SoftDebuggerStartInfo
	{
		public MonoBrickExecutionCommand ExecutionCommand { get; private set; }

		public MonoBrickSoftDebuggerStartInfo(IPAddress address, int debugPort, MonoBrickExecutionCommand cmd)
			: base(new SoftDebuggerListenArgs(cmd.AppName, address, debugPort))
		{
			ExecutionCommand = cmd;
		}
	}
}