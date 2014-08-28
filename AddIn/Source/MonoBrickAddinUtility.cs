// MonoBrickAddinUtility.cs
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
using MonoDevelop.Core.Execution;
using MonoDevelop.Core;
using Renci.SshNet;
using System.Text;

namespace MonoBrickAddin
{
	class MonoBrickUtility
	{
		public static void ClearScreen(string IPAddress)
		{
			int Width = 178;
			int Height = 128;
			int bytesPrLine = ((Width+31)/32)*4;
			int bufferSize = bytesPrLine * Height;
			var handle = new System.Threading.ManualResetEvent(false);
			var helper = new SshCommandHelper(IPAddress, handle);
			StringBuilder lcdStringBuilder = new StringBuilder();
			for (uint i = 0; i < bufferSize; i++)
				lcdStringBuilder.Append("1");
			helper.WriteSSHCommand("echo " + lcdStringBuilder.ToString() + @" >/dev/tb0", true);
			handle.WaitOne();
		}


		public static void KillMonoApp(string IPAddress)
		{
			var handle = new System.Threading.ManualResetEvent(false);
			var helper = new SshCommandHelper(IPAddress, handle);
			helper.WriteSSHCommand("kill -9 `ps | grep mono | awk '{{print $1}}'`", true);
			handle.WaitOne();
		}


		public static ScpUpload Upload(string IPAddress, MonoBrickExecutionCommand cmd)
		{
			var scp = new ScpUpload(IPAddress, cmd.Config.OutputDirectory, cmd.DeviceDirectory);
			scp.Upload();
			return scp;
		}

		public static SshExecute ExecuteCommand(string IPAddress, MonoBrickExecutionCommand cmd, string sdbOptions, IConsole console, bool verbose)
		{
			var sshC = new SshExecute(IPAddress, cmd, sdbOptions, console, verbose);
			return sshC;
		}

		public static bool GetEV3Configuration(IConsole console)
		{

			if (UserSettings.Instance.IPAddress == "0" ||
				UserSettings.Instance.DebugPort == "")
				return false;

			string EV3IPAddress = UserSettings.Instance.IPAddress;
			string EV3DebuggerPort = UserSettings.Instance.DebugPort;

			bool bValid = true;

			try
			{
				IPAddress.Parse(EV3IPAddress);
				int.Parse(EV3DebuggerPort);
			}
			catch (Exception ex)
			{
				console.Log.WriteLine(ex.Message);
				bValid = false;
			}

			return bValid;
		}
	}
}