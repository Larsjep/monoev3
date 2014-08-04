using System;
using System.Net;
using System.IO;
using MonoBrickFirmware.Tools;
using MonoBrickFirmware.Extensions;

namespace StartupApp
{
	public class UpdateHelper
	{
		private static string DownloadURL = "http://www.monobrick.dk/MonoBrickFirmwareRelease/Test/";
		private static string StartUPAppName = "StartupApp.exe";
		private static string FirmwareDllName = "MonoBrickFirmware.dll";
		private static string XmlSerializersName = "XmlSerializers.dll";
		private static string StartupFile = "startup";
		private static string BinDir = @"/usr/local/bin";
		private string newDir;

		public UpdateHelper(string version)
		{
			newDir = Path.Combine (BinDir, version);
		}

		private bool DownloadFile(string file, string url, string downloadPath)
		{
			bool ok = true;
			try
			{
				if(!Directory.Exists(downloadPath))
					Directory.CreateDirectory(downloadPath);
				new WebClient ().DownloadFile (url + file, Path.Combine(downloadPath,file));
			}
			catch
			{
				ok = false;
			}
			return ok;
		}

		public bool DownloadFirmware()
		{
			bool ok = true;
			string[] filesToDownload = new string[]{ StartUPAppName, FirmwareDllName, XmlSerializersName };
			foreach (var file in filesToDownload) 
			{
				if (!DownloadFile (file, DownloadURL, newDir)) 
				{
					ok = false;
					break;
				}
			}
			return ok;
		}

		public bool UpdateBootFile()
		{
			bool ok = true;
			try{
				string[] lines = File.ReadAllLines(StartupFile);
				string[] checkFor = new string[]{ StartUPAppName, FirmwareDllName, XmlSerializersName };
				for(int i = 0; i < lines.Length; i++)
				{
					for (int k = 0; k < checkFor.Length; k++) 
					{
						if (lines [i].Contains (checkFor [k])) 
						{
							string[] commands = lines [i].Split (' ');
							for (int j = 0; j < commands.Length; j++) 
							{
								if(commands[j].Contains(BinDir) && commands[j].Contains(checkFor[k]))
								{
									commands [j] = commands [j].Before (BinDir) + newDir + @"/" + checkFor [k] + commands [j].After (checkFor [k]);
								}
							}
							string newString = "";
							for (int j = 0; j < commands.Length; j++) 
							{
								newString = newString + commands[j] + " ";
							}
							lines [i] = newString;
						}
					}
				}
				File.WriteAllLines(StartupFile, lines);
				/*for (int i = 0; i < lines.Length; i++) 
				{
					Console.WriteLine (lines [i]);
				}*/
			}
			catch
			{
				ok = false;
			}
			return ok;
		} 
	}
}

