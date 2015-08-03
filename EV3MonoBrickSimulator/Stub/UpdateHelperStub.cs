using System;
using MonoBrickFirmware.FirmwareUpdate;
using System.Reflection;
using System.IO;
using MonoBrickFirmware.Extensions;
using EV3MonoBrickSimulator.Settings;

namespace EV3MonoBrickSimulator.Stub
{
	internal class UpdateHelperStub : EV3UpdateHelper
	{
		public UpdateHelperStub()
		{
			BinDir = Directory.GetCurrentDirectory ();
		}

		public string ImageVersion{ get; set;}

		public string AddInVersion{ get; set;}

		public string Repository{ get; set;}

		public override bool UpdateBootFile ()
		{
			string newDir = Path.Combine (Directory.GetCurrentDirectory(), GetAvailableFirmware()); 
			SimulatorSettings settings = new SimulatorSettings ();
			if(settings.Load ())
			{
				settings.BootSettings.StartUpDir = newDir;
				return settings.Save ();
			}
			return false;
		}

		protected override string GetRepository ()
		{
			return Repository;
		}

		protected override string CurrentImageVersion ()
		{
			return ImageVersion;
		}

		protected override string CurrentAddInVersion ()
		{
			return AddInVersion;	
		}

		protected override string CurrentFirmwareVersion ()
		{
			SimulatorSettings settings = new SimulatorSettings ();
			settings.Load ();
			string runningPath = settings.BootSettings.StartUpDir;
			return Assembly.LoadFrom(Path.Combine(runningPath, "MonoBrickFirmware.dll")).GetName().Version.ToString();
		}

	}
}

