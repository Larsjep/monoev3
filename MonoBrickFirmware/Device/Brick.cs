using System;

namespace MonoBrickFirmware.Device
{
	public static class Brick
	{

		static Brick()
		{
			Instance = new EV3Brick ();
		}

		internal static IBrick Instance{ get; set;}

		public static float BatteryCurrent (){return Instance.BatteryCurrent ();}
		public static float BatteryVoltage(){return Instance.BatteryVoltage ();}
		public static void TurnOff(){Instance.TurnOff ();}
	}
}

