using System;
using System.Diagnostics;
using System.IO;
namespace MonoBrickFirmware.Native
{
	public class ProcessHelper
	{
		public static int RunAndWaitForProcess(string fileName, string arguments = "" , int timeout = 0){
			Process proc = new System.Diagnostics.Process ();
			System.Timers.Timer timer = new System.Timers.Timer(timeout);
			if(timeout != 0){
				timer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e) {
					timer.Stop();
					proc.Kill();
				};
				timer.Start();	
			}
			proc.EnableRaisingEvents = false; 
			Console.WriteLine ("Starting process: {0} with arguments: {1}", fileName, arguments);
			proc.StartInfo.FileName = fileName;
			proc.StartInfo.Arguments = arguments;
			proc.Start ();
			proc.WaitForExit ();
			return proc.ExitCode;	
		}
		
		public static string RunAndWaitForProcessWithOutput(string fileName, string arguments = "")
		{
			string result;
			ProcessStartInfo start = new ProcessStartInfo();
			start.FileName = fileName;
			start.Arguments = arguments; 
			start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			Console.WriteLine ("Starting process: {0} with arguments: {1}", fileName, arguments);
			using (Process process = Process.Start(start))
			{
			    using (StreamReader reader = process.StandardOutput)
			    {
					result = reader.ReadToEnd();
				}
				process.WaitForExit();
			}
			return result;
		} 
	}
}

