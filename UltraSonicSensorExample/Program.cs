using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace UltraSonicSensorExample.IO
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent (false);
			ButtonEvents buts = new ButtonEvents ();
			var sensor = new UltraSonicSensor(SensorPort.In1, UltraSonicMode.Centimeter);
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
