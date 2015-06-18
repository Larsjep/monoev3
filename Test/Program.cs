using System;
using MonoBrickFirmware.FirmwareUpdate;

namespace Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var aVersion = VersionHelper.AvailableVersions ();
			Console.WriteLine ("Available version");
			Console.WriteLine ("Addin: " + aVersion.AddIn);
			Console.WriteLine ("Firmware: " + aVersion.Firmware);
			Console.WriteLine ("Image: " + aVersion.Image);

			var cVersion = VersionHelper.InstalledVersion ();
			Console.WriteLine ("Installed version");
			Console.WriteLine ("Addin: " + cVersion.AddIn);
			Console.WriteLine ("Firmware: " + cVersion.Firmware);
			Console.WriteLine ("Image: " + cVersion.Image);


		}
	}
}
