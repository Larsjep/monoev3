using System;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;
namespace TouchSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			bool run = true;
			var touchSensor = new TouchSensor(SensorPort.In1);
			Buttons buts = new Buttons ();
			touchSensor.Initialize();
			buts.EnterPressed += () => { 
				run  = false;
			};
			buts.UpPressed += () => { 
				Console.WriteLine("Sensor value:" + touchSensor.ReadAsString());
			};
			buts.DownPressed += () => { 
				if(touchSensor.Mode == TouchMode.Boolean){
					touchSensor.Mode = TouchMode.Raw;
				}
				else{
					touchSensor.Mode = TouchMode.Boolean;
				}
				Console.WriteLine("Sensor mode is now: " + touchSensor.Mode);
			};  
			while (run) {
				System.Threading.Thread.Sleep(50);
			}
		}
	}
}
