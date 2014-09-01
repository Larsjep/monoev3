using System;
using System.Net;
namespace MonoBrickFirmware.Tools
{
	public class VersionInfo
	{
		public VersionInfo(string firmware, string image, string addIn)
		{
			Fimrware = firmware;
			Image = image;
			AddIn = addIn;
		}

		public string Fimrware{ get; private set;}
		public string Image{ get; private set;}
		public string AddIn{ get; private set;}
	}

	public static class VersionHelper
	{
		private static string versionURL = "http://www.monobrick.dk/MonoBrickFirmwareRelease/latest/version.txt";
		private static string versionPath = @"/usr/local/bin/version.txt";
		private static string addInVersionPath = @"/usr/local/bin/add-inVersion.txt";


		public static VersionInfo AvailableVersions()
		{
			VersionInfo info = null;
			try{
				string[] downloadInfo = new WebClient ().DownloadString (versionURL).Split (new char[] {'\n' });
				info = new VersionInfo(downloadInfo[0].Split(new char[] {':'})[1].Trim(),downloadInfo[1].Split(new char[] {':'})[1].Trim(), downloadInfo[2].Split(new char[] {':'})[1].Trim());
			}
			catch{}
			return info;
		}

		public static string CurrentImageVersion()
		{
			return System.IO.File.ReadAllLines(versionPath)[1].Split(new char[] {':'})[1].Trim();
		}

		public static string CurrentAddInVersion ()
		{
			string val = null;
			try 
			{
				val = System.IO.File.ReadAllLines (addInVersionPath) [0].Split (new char[] { ':' }) [1].Trim ();
			} 
			catch 
			{
			}
			return val;
		}
	}
}

