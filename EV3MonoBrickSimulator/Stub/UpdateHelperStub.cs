using System;
using MonoBrickFirmware.FirmwareUpdate;
using System.Reflection;
using System.IO;
using MonoBrickFirmware.Extensions;
using EV3MonoBrickSimulator.Settings;
using System.Xml;

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
			/* Not yet used need some building 
			XmlDocument doc = new XmlDocument();
			doc.Load("EV3MonoBrickSimulator.exe.config");
			XmlNode node = doc.SelectSingleNode ("configuration/runtime").FirstChild.FirstChild;
			node.Attributes [0].Value = GetAvailableFirmware ();
			doc.Save ("EV3MonoBrickSimulator.exe.config");
			*/

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

