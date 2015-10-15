using System;
using System.Net;
using System.IO;
using MonoBrickFirmware.Extensions;

namespace MonoBrickFirmware.FirmwareUpdate
{
	public static class UpdateHelper
	{
		static UpdateHelper()
		{
			Instance = new EV3UpdateHelper ();
		}
		internal static IUpdateHelper Instance{ get; set;} 
		public static VersionInfo AvailableVersions (){return Instance.AvailableVersions();}
		public static VersionInfo InstalledVersion(){return Instance.InstalledVersion ();}
		public static bool DownloadFirmware (){return Instance.DownloadFirmware ();}
		public static bool UpdateBootFile(){return Instance.UpdateBootFile ();}
	}
}

