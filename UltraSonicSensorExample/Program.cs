using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;

namespace UltraSonicSensorExample.IO
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent (false);
			ButtonEvents buts = new ButtonEvents ();
			var sensor = new UltraSonicSensor(SensorPort.In1, UltraSonicMode.Centimeter);
			LcdConsole.WriteLine("Use sonic on port1");
			LcdConsole.WriteLine("Enter read");
			LcdConsole.WriteLine("Esc. terminate");
			buts.EscapePressed += () => { 
				terminateProgram.Set ();
			};
			buts.EnterPressed += () => {
				LcdConsole.WriteLine ("Distance: " + sensor.ReadDistance());
			};
			terminateProgram.WaitOne ();  
		}
	}
}
