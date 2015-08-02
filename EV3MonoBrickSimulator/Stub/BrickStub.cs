using System;
using MonoBrickFirmware.Device;

namespace EV3MonoBrickSimulator
{
	public class BrickStub : IBrick
	{
		public event Action OnShutDown;

		public int TurnOffTimeMs{ get; set;}

		public float BatteryCurrent ()
		{
			return 0.75f;
		}

		public float BatteryVoltage ()
		{
			return 7.5f;
		}

		public void TurnOff ()
		{
			System.Threading.Thread.Sleep (TurnOffTimeMs);
			OnShutDown ();
		}

		public BrickStub ()
		{
			OnShutDown += delegate {};
		}

	}
}

