using System;
using MonoBrickFirmware.FirmwareUpdate;
namespace EV3MonoBrickSimulator.Stub
{
	internal class VersionHelperStub : EV3VersionHelper
	{
		public VersionHelperStub ()
		{
			StartupFile = @"Stub/StartUp.sh";
		}

		public string ImageVersion{ get; set;}

		public string AddInVersion{ get; set;}

		public string Repository{ get; set;}

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

	}
}

