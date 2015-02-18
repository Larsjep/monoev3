﻿using System;
using System.Net;
using System.IO;
using MonoBrickFirmware.Tools;
using MonoBrickFirmware.Extensions;

namespace StartupApp
{
	public class UpdateHelper
	{
		private static string PackageURL = "http://www.monobrick.dk/MonoBrickFirmwareRelease/latest/firware/";
		private static string PackageName = "package.xml";
		private static string StartUPAppName = "StartupApp.exe";
		private static string FirmwareDllName = "MonoBrickFirmware.dll";
		private static string XmlSerializersName = "StartupApp.XmlSerializers.dll";
		private static string StartupFile = @"/home/root/lejos/bin/startup";
		private static string BinDir = @"/usr/local/bin";
		private string newDir;

		public UpdateHelper(string version)
		{
			newDir = Path.Combine (BinDir, version);
		}

		private static bool DownloadFile(string file, string url, string downloadPath, bool overwriteFiles)
		{
			bool ok = true;
			try
			{
				if(!Directory.Exists(downloadPath))
				{
					Directory.CreateDirectory(downloadPath);
				}
				string downloadFileName = Path.Combine(downloadPath,file);
				if(overwriteFiles || (!overwriteFiles && !File.Exists(downloadFileName)))
				{
					new WebClient ().DownloadFile (Path.Combine(url,file), downloadFileName );
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				ok = false;
			}
			return ok;
		}

		private static bool DownloadPackage(string packageName, string packageUrl, string downloadPath, bool overwriteFiles)
		{
			bool ok = true;
			InstallPackage installPackage = new InstallPackage();
			ok = DownloadFile(packageName, packageUrl, downloadPath, true);
			if(ok)
			{
				try
				{
					installPackage = installPackage.LoadFromXML(Path.Combine(downloadPath,packageName));
				}
				catch
				{
					ok = false;
				}
				if(ok)
				{
					var downloadElements = installPackage.DownloadElementToArray();
					foreach(var element in downloadElements)
					{
						ok = DownloadFile(element.FileName, Path.Combine(element.Url, element.Subdir), Path.Combine(downloadPath, element.Subdir), overwriteFiles);
						if(!ok)
						{
							break;
						}
					}
				}
			}
			return ok;
		}

		public bool DownloadMissingFiles ()
		{
			return DownloadPackage(PackageName, PackageURL, newDir, false);		
		}

		public bool DownloadFirmware()
		{
			return DownloadPackage(PackageName, PackageURL, newDir, true);
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
			}
			catch
			{
				ok = false;
			}
			return ok;
		} 
	}
}

