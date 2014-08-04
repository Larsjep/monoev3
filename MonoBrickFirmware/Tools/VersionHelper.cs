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
		private static string versionURL = "http://www.monobrick.dk/MonoBrickFirmwareRelease/Test/version.txt";
		private static string versionImagePath = "http://www.monobrick.dk/MonoBrickFirmwareRelease/latest/version.txt";

		public static VersionInfo AvalibleVersions()
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
			return "1.0.0.0";//read this from a file
		}

		public static string CurrentAddInVersion()
		{
			return "1.0.0.0";//read this from a file
		}
	}
}

