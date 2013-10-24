using System;
using MonoBrickFirmware;
using MonoBrickFirmware.IO;
namespace ColorSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ColorMode[] modes = {ColorMode.Ambient,ColorMode.Color, ColorMode.NXTBlue, 
								ColorMode.NXTGreen, ColorMode.Reflection};
			int modeIdx = 0;
			bool run = true;
			var colorSensor = new NXTColorSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			colorSensor.Mode = modes[modeIdx];
			buts.EnterPressed += () => { 
				run  = false;
			};
			buts.UpPressed += () => { 
				Console.WriteLine("Sensor value: " + colorSensor.ReadAsString());
			};
			buts.LeftPressed += () => { 
				Console.WriteLine("Raw sensor value: " + colorSensor.ReadRaw());
			};
			buts.RightPressed += () => { 
				RGBColor RGB = colorSensor.ReadRGBColor();
				Console.WriteLine("Red value  : " + RGB.Red);
				Console.WriteLine("Green value: " + RGB.Green);
				Console.WriteLine("Blue value : " + RGB.Blue);
			};
			buts.DownPressed += () => { 
				colorSensor.Mode = modes[(modeIdx+1)%modes.Length];
				modeIdx = (modeIdx+1)%modes.Length;
				Console.WriteLine("Sensor mode is now: " + colorSensor.Mode);
			};  
			while (run) {
				System.Threading.Thread.Sleep(50);
			}
		}
	}
}
