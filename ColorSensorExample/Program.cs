using System;
using MonoBrickFirmware;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;
using System.Threading;
namespace NXTColorSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			EventWaitHandle stopped = new ManualResetEvent(false);
			ColorMode[] modes = {ColorMode.Color, ColorMode.Reflection, ColorMode.Ambient, ColorMode.Blue};
			int modeIdx = 0;
			var colorSensor = new ColorSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			
			buts.EscapePressed += () => { 
				stopped.Set();
			};
			buts.UpPressed += () => { 
				LcdConsole.WriteLine("Sensor value: " + colorSensor.ReadAsString());
			};
			buts.DownPressed += () => { 
				LcdConsole.WriteLine("Raw sensor value: " + colorSensor.ReadRaw());
			};
			buts.EnterPressed += () => { 
				modeIdx = (modeIdx+1)%modes.Length;
				colorSensor.Mode = modes[modeIdx];
				LcdConsole.WriteLine("Sensor mode is set to: " + modes[modeIdx]);
			};  
			stopped.WaitOne();
		}
	}
}
