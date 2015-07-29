using System;
using System.Net;
using System.IO;
using System.Reflection;

namespace MonoBrickFirmware.FirmwareUpdate
{
	internal class EV3VersionHelper : IVersionHelper
	{
		protected string imageVersionPath = @"/usr/local/bin/version.txt";
		protected string addInVersionPath = @"/usr/local/bin/add-inVersion.txt";
		protected string repositoryFile = @"/usr/local/bin/repository.txt";
		protected string StartupFile = @"/home/root/lejos/bin/startup";
		protected string repository = null;

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

		protected virtual string GetRepository()
		{
			if (repository == null)
			{
				repository = System.IO.File.ReadAllLines (repositoryFile)[0];		
			}
			return repository;

		}

		private string GetAvailableImage()
		{
			string repo = GetRepository ();
			string image = new WebClient ().DownloadString (repo + "images/version.txt").Trim();
			return image;
		}

		private string GetAvailableFirmware()
		{
			string repo = GetRepository ();
			string firmware =  new WebClient ().DownloadString (repo + "version.txt").Trim();
			return firmware;
		}

		protected virtual string CurrentFirmwareVersion()
		{

			string dllPath = null;
			string[] lines = File.ReadAllLines(StartupFile);
			foreach (var s in lines) 
			{
				if (s.Contains ("MonoBrickFirmware.dll")) 
				{
					string[] commands = s.Split (' ');
					foreach (var command in commands)
					{
						if(command.Contains("MonoBrickFirmware.dll"))
						{
							dllPath = command;
							goto Done;
						}	
					}
				}
			}
			FoundPath: 
			return Assembly.LoadFrom(dllPath).GetName().Version.ToString();
		}

		protected virtual string CurrentImageVersion()
		{
			return System.IO.File.ReadAllLines(imageVersionPath)[0].Trim();
		}

		protected virtual string CurrentAddInVersion ()
		{
			string val = null;
			try 
			{
				val = System.IO.File.ReadAllLines (addInVersionPath) [0].Trim ();
			} 
			catch 
			{

			}
			return val;
		}
	}
}

