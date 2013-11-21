using System;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;
using System.Threading;

namespace PCF8574Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent(false);
			ButtonEvents buts = new ButtonEvents ();
			PCF8574 sensor = new PCF8574(SensorPort.In1,0x20);
			buts.EscapePressed += () => { 
				terminateProgram.Set();
			};
			buts.UpPressed += () => { 
				LcdConsole.WriteLine(sensor.ReadAsString());
			};
			buts.DownPressed += () => { 
				LcdConsole.WriteLine("Writing 0x20");
				sensor.Write(0x20);
			};
			terminateProgram.WaitOne();  
		}
	
		
	}
}
