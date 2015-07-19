using System;

namespace MonoBrickFirmware.FirmwareUpdate
{
	public interface IVersionHelper
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
	}
}

