using System;
using MonoBrickFirmware.Settings;

namespace MonoBrickFirmwareSimulation.Mock
{
	public class SettingsMock : EV3FirmwareSettings
	{
		public SettingsMock ()
		{
			this.SettingsFileName = "FirmwareSettings.xml";
		}
	}
}

