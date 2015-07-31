using System;

namespace MonoBrickFirmware.FirmwareUpdate
{
	public interface IUpdateHelper
	{

		/// <summary>
		/// Get a version info class containing what is curently available.
		/// </summary>
		/// <returns>The version.</returns>
		VersionInfo AvailableVersions ();

		/// <summary>
		/// Get a version info class containing what is curently installed.
		/// If no program has been uploaded using the addIn then Addin strin will be null 
		/// </summary>
		/// <returns>The version.</returns>
		VersionInfo InstalledVersion();


		/// <summary>
		/// Downloads the firmware 
		/// </summary>
		/// <returns><c>true</c>, if firmware was downloaded, <c>false</c> otherwise.</returns>
		bool DownloadFirmware ();

		/// <summary>
		/// Updates the boot file.
		/// </summary>
		/// <returns><c>true</c>, if boot file was updated, <c>false</c> otherwise.</returns>
		bool UpdateBootFile();
	}
}

