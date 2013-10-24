using System;
using MonoBrickFirmware.IO;
namespace TouchSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			bool run = true;
			var touchSensor = new TouchSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			buts.EnterPressed += () => { 
				run  = false;
			};
			buts.UpPressed += () => { 
				Console.WriteLine("Sensor value:" + touchSensor.ReadAsString());
			};
			buts.DownPressed += () => { 
				Console.WriteLine("Raw sensor value: " + touchSensor.ReadRaw());
			};  
			while (run) {
				System.Threading.Thread.Sleep(50);
			}
		}
	}
}
