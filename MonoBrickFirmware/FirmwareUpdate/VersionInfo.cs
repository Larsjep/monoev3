using System;

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
}

