using System;

namespace MonoBrickFirmware.Device
{
	public interface IBrick
	{
		float BatteryCurrent ();
		float BatteryVoltage ();
		void TurnOff();
	}
}

