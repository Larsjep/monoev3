// MonoBrickAddinExecutionHandler.cs
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
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;

namespace MonoBrickAddin
{
	class MonoBrickExecutionHandler : IExecutionHandler
	{
		public bool AOT { get; private set; }

		public MonoBrickExecutionHandler()
		{
			AOT = false;
		}

		public MonoBrickExecutionHandler(bool _aot)
		{
			AOT = _aot;
		}

		public bool CanExecute(ExecutionCommand command)
		{
			MonoBrickExecutionCommand cmd = command as MonoBrickExecutionCommand;
			if (cmd == null)
				return false;

			if (AOT && cmd.Config.Name != "Release")
				return false;

			return true;
		}

		public IProcessAsyncOperation Execute(ExecutionCommand command, IConsole console)
		{
			var cmd = command as MonoBrickExecutionCommand;
			cmd.AOT = AOT;
			string EV3IPAddress = UserSettings.Instance.IPAddress;
			bool EV3Verbose = UserSettings.Instance.Verbose;

			var proc = MonoBrickUtility.ExecuteCommand(EV3IPAddress, cmd, null, EV3Verbose);
			proc.Execute();
			return proc;
		}
	}
}
