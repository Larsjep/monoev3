using System;

namespace MonoBrickFirmware.FirmwareUpdate
{

	public static class VersionHelper
	{
		static VersionHelper()
		{
			Instance = new EV3VersionHelper ();	
		}


		internal static IVersionHelper Instance{ get; set;}


		/// <summary>
		/// Get a version info class containing what is curently available.
		/// </summary>
		/// <returns>The version.</returns>
		public static VersionInfo AvailableVersions (){return Instance.AvailableVersions();}

		/// <summary>
		/// Get a version info class containing what is curently installed.
		/// If no program has been uploaded using the addIn then Addin strin will be null 
		/// </summary>
		/// <returns>The version.</returns>
		public static VersionInfo InstalledVersion(){return Instance.InstalledVersion();}
	} 
}

