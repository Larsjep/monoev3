using System;
using System.Net;
namespace MonoBrickFirmware.Tools
{
	public static class VersionHelper
	{
		private static string versionURL = "http://www.monobrick.dk/MonoBrickFirmwareRelease/latest/version.txt";

		private static string[] ReadURLVersion(string url)
		{
			return new WebClient ().DownloadString (url).Split (new char[] { '\r', '\n' });
		}

		public static string CurrentImageVersion()
		{
			return "Image: 1.0.0.0";//read this from a file
		}

		public static string AvailableImageVersion()
		{
			return ReadURLVersion (versionURL)[1];
		}

		public static string CurrentPluginVersion()
		{
			return "Plugin: 1.0.0.0";//read this from a file
		}

		public static string AvailablePluginVersion()
		{
			return ReadURLVersion (versionURL)[2];
		}

		public static string AvailableFirmwareApp()
		{
			return ReadURLVersion (versionURL)[0];
		}
	}
}

