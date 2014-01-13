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
			UltraSonicMode[] modes = {UltraSonicMode.Centimeter, UltraSonicMode.Inch, UltraSonicMode.Listen};
			int modeIdx = 0;
			ButtonEvents buts = new ButtonEvents ();
			var sensor2 = new UltraSonicSensor(SensorPort.In2, UltraSonicMode.Centimeter);
			sensor2.Continuous();
			var sensor = new EV3UltrasonicSensor(SensorPort.In1, modes[modeIdx]);
			LcdConsole.WriteLine("Use sonic on port1");
			LcdConsole.WriteLine("Up change mode");
			LcdConsole.WriteLine("Enter read");
			LcdConsole.WriteLine("Esc. terminate");
			buts.EscapePressed += () => { 
				terminateProgram.Set ();
			};
			buts.UpPressed += () => { 
				modeIdx = (modeIdx+1)%modes.Length;
				sensor.Mode = modes[modeIdx];
				LcdConsole.WriteLine("Mode: " + modes[modeIdx]);
			};
			buts.EnterPressed += () => {
				LcdConsole.WriteLine (sensor.ReadAsString());
			};
			terminateProgram.WaitOne ();  
		}
	}
}
