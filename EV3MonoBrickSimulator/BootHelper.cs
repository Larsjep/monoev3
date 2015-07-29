using System;
using System.IO;

namespace EV3MonoBrickSimulator
{
	public static class BootHelper
	{
		public static string GetStartUpAppPath()
		{
			string startUpAppPath = null;
			string[] lines = File.ReadAllLines(@"Stub/StartUp.sh");
			foreach (var s in lines) 
			{
				if (s.Contains ("StartupApp.exe")) 
				{
					string[] commands = s.Split (' ');
					foreach (var command in commands)
					{
						if(command.EndsWith("StartupApp.exe"))
						{
							startUpAppPath = command;
							goto Done;
						}	
					}
				}
			}
			Done:
			if(!Directory.Exists(startUpAppPath))
			{
				startUpAppPath = System.IO.Path.Combine(Directory.GetCurrentDirectory (), "StartupApp.exe");
			}
			return startUpAppPath;
		}
	}
}

