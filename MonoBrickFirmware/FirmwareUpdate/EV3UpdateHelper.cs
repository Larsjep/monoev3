using System;
using System.IO;
using System.Net;
using System.Reflection;
using MonoBrickFirmware.Extensions;

namespace MonoBrickFirmware.FirmwareUpdate
{
	internal class EV3UpdateHelper : IUpdateHelper
	{
		private  string imageVersionPath = @"/usr/local/bin/version.txt";
		private  string addInVersionPath = @"/usr/local/bin/add-inVersion.txt";
		private  string repositoryFile = @"/usr/local/bin/repository.txt";
		private  string StartupFile = @"/home/root/lejos/bin/startup";
		private  string RepositoryURL = null;
		private  string PackageName = "package.xml";
		private  string StartUPAppName = "StartupApp.exe";
		private  string FirmwareDllName = "MonoBrickFirmware.dll";
		private  string XmlSerializersName = "MonoBrickFirmware.XmlSerializers.dll";
		private string availableFirmware = null;
		private string availableImage = null;
		protected string BinDir = @"/usr/local/bin";

		public bool DownloadFirmware ()
		{
			return DownloadPackage(PackageName,Path.Combine(GetRepository(), "StartupApp"), Path.Combine(BinDir, GetAvailableFirmware()), true);
		}

		public virtual bool UpdateBootFile ()
		{
			string newDir = Path.Combine (BinDir, GetAvailableFirmware()); 
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

		public VersionInfo AvailableVersions()
		{
			string image = GetAvailableImage ();
			string firmware = GetAvailableFirmware ();
			return new VersionInfo (firmware, image, firmware);
		}

		public VersionInfo InstalledVersion()
		{
			string firmware = CurrentFirmwareVersion (); 
			string image = CurrentImageVersion();
			string addIn = CurrentAddInVersion ();
			return new VersionInfo(firmware, image, addIn);
		}

		private bool DownloadFile(string file, string url, string downloadPath, bool overwriteFiles)
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
					string urlDownload = Path.Combine(url,file);
					Console.WriteLine(urlDownload);
					new WebClient ().DownloadFile (urlDownload, downloadFileName );
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				ok = false;
			}
			return ok;
		}

		private bool DownloadPackage(string packageName, string packageUrl, string downloadPath, bool overwriteFiles)
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
						ok = DownloadFile(element.FileName, Path.Combine(Path.Combine(GetRepository(),"StartupApp") , element.Subdir), Path.Combine(downloadPath, element.Subdir), overwriteFiles);
						if(!ok)
						{
							break;
						}
					}
				}
			}
			return ok;
		}


		protected virtual string GetRepository()
		{
			if (RepositoryURL == null)
			{
				//repository = File.ReadAllText (RepositoryFile);
				RepositoryURL = File.ReadAllLines (repositoryFile)[0];		
			}
			return RepositoryURL;

		}

		protected virtual string CurrentFirmwareVersion()
		{

			string dllPath = null;
			string[] lines = File.ReadAllLines(StartupFile);
			foreach (var s in lines) 
			{
				if (s.Contains (FirmwareDllName)) 
				{
					string[] commands = s.Split (' ');
					foreach (var command in commands)
					{
						if(command.Contains(FirmwareDllName))
						{
							dllPath = command;
							goto FoundPath;
						}	
					}
				}
			}
			FoundPath: 
			return Assembly.LoadFrom(dllPath).GetName().Version.ToString();
		}

		protected virtual string CurrentImageVersion()
		{
			return File.ReadAllLines(imageVersionPath)[0].Trim();
		}

		protected virtual string CurrentAddInVersion ()
		{
			string val = null;
			try 
			{
				val = File.ReadAllLines (addInVersionPath) [0].Trim ();
			} 
			catch 
			{

			}
			return val;
		}

		private string GetAvailableImage()
		{
			if (availableImage == null)
			{
				string repo = GetRepository ();
				availableImage = new WebClient ().DownloadString (repo + "images/version.txt").Trim();

			}
			return availableImage;
		}

		protected string GetAvailableFirmware()
		{
			if (availableFirmware == null)
			{
				string repo = GetRepository ();
				availableFirmware =  new WebClient ().DownloadString (repo + "version.txt").Trim();
			}
			return availableFirmware;
		}

	}
}

