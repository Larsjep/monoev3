using System;
using MonoBrickFirmware.IO;
namespace LightSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			bool run = true;
			var lightSensor = new LightSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			lightSensor.Initialize();
			buts.EnterPressed += () => { 
				run  = false;
			};
			buts.UpPressed += () => { 
				Console.WriteLine("Sensor value:" + lightSensor.ReadAsString());
			};
			buts.DownPressed += () => { 
				if(lightSensor.Mode == LightMode.Ambient){
					lightSensor.Mode = LightMode.Relection;
				}
				else{
					lightSensor.Mode = LightMode.Ambient;
				}
				Console.WriteLine("Sensor mode is now: " + lightSensor.Mode);
			};  
			while (run) {
				System.Threading.Thread.Sleep(50);
			}
		}
	}
}
