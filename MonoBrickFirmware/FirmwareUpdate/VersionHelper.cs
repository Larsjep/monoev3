using System;
using System.Net;
using System.Reflection;
using System.IO;


namespace MonoBrickFirmware.FirmwareUpdate
{
	public class VersionInfo
	{
		public VersionInfo(string firmware, string image, string addIn)
		{
			Firmware = firmware;
			Image = image;
			AddIn = addIn;
		}

		public string Firmware{ get; private set;}
		public string Image{ get; private set;}
		public string AddIn{ get; private set;}
	}

	public static class VersionHelper
	{
		private const string imageVersionPath = @"/usr/local/bin/version.txt";
		private const string addInVersionPath = @"/usr/local/bin/add-inVersion.txt";
		private const string repositoryFile = @"/usr/local/bin/repository.txt";
		private const string StartupFile = @"/home/root/lejos/bin/startup";
		private static string repository = null;

		public static VersionInfo AvailableVersions()
		{
			string image = GetAvailableImage ();
			string firmware = GetAvailableFirmware ();
			return new VersionInfo (firmware, image, firmware);
		}

		/// <summary>
		/// Get a version info class containing what is curently installed.
		/// If no program has been uploaded with the addIn then Addin strin will be null 
		/// </summary>
		/// <returns>The version.</returns>
		public static VersionInfo InstalledVersion()
		{
			string firmware = CurrentFirmwareVersion (); 
			string image = CurrentImageVersion();
			string addIn = CurrentAddInVersion ();
			return new VersionInfo(firmware, image, addIn);
		}

		private static string GetRepository()
		{
			if (repository == null)
			{
				repository = System.IO.File.ReadAllLines (repositoryFile)[0];		
			}
			return repository;
		
		}

		private static string GetAvailableImage()
		{
			string repo = GetRepository ();
			string image = new WebClient ().DownloadString (repo + "images/version.txt").Trim();
			return image;
		}

		private static string GetAvailableFirmware()
		{
			string repo = GetRepository ();
			string firmware =  new WebClient ().DownloadString (repo + "version.txt").Trim();
			return firmware;
		}

		private static string CurrentFirmwareVersion()
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
							break;
						}	
					}
				}
			}
			return Assembly.LoadFrom(dllPath).GetName().Version.ToString();
		}

		private static string CurrentImageVersion()
		{
			return System.IO.File.ReadAllLines(imageVersionPath)[0].Trim();
		}

		private static string CurrentAddInVersion ()
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

