using System;
using MonoBrickFirmware.IO;
namespace EV3ColorSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			EV3ColorMode[] modes = {EV3ColorMode.Color, EV3ColorMode.Reflection, EV3ColorMode.Ambient};
			int modeIdx = 0;
			bool run = true;
			var colorSensor = new EV3ColorSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			
			buts.EnterPressed += () => { 
				run  = false;
			};
			buts.UpPressed += () => { 
				Console.WriteLine("Sensor value: " + colorSensor.ReadAsString());
			};
			buts.DownPressed += () => { 
				modeIdx = (modeIdx+1)%modes.Length;
				colorSensor.Mode = modes[modeIdx];
				Console.WriteLine("Sensor mode is now: " + colorSensor.Mode);
			};  
			while (run) {
				System.Threading.Thread.Sleep(50);
			}
		}
	}
}
