using System;
using MonoBrickFirmware;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;
namespace NXTColorSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ColorMode[] modes = {ColorMode.Color, ColorMode.Reflection, ColorMode.Ambient, ColorMode.Blue, ColorMode.Green};
			int modeIdx = 0;
			bool run = true;
			var colorSensor = new ColorSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			
			buts.EnterPressed += () => { 
				run  = false;
			};
			buts.UpPressed += () => { 
				LcdConsole.WriteLine("Sensor value: " + colorSensor.ReadAsString());
			};
			buts.LeftPressed += () => { 
				LcdConsole.WriteLine("Raw sensor value: " + colorSensor.ReadRaw());
			};
			buts.RightPressed += () => { 
				RGBColor RGB = colorSensor.ReadRGBColor();
				LcdConsole.WriteLine("Red value  : " + RGB.Red);
				LcdConsole.WriteLine("Green value: " + RGB.Green);
				LcdConsole.WriteLine("Blue value : " + RGB.Blue);
			};
			buts.DownPressed += () => { 
				modeIdx = (modeIdx+1)%modes.Length;
				colorSensor.Mode = modes[modeIdx];
				LcdConsole.WriteLine("Sensor mode is set to: " + modes[modeIdx]);
			};  
			while (run) {
				System.Threading.Thread.Sleep(50);
			}
		}
	}
}
