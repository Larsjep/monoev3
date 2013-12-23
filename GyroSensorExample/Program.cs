using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;

namespace GyroSensorExample.IO
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent (false);
			ButtonEvents buts = new ButtonEvents ();
			var gyro = new HiTecGyroSensor(SensorPort.In1, 600);
			LcdConsole.WriteLine("Use gyro on port1");
			LcdConsole.WriteLine("Enter read value");
			LcdConsole.WriteLine("Esc. terminate");
			buts.EscapePressed += () => { 
				terminateProgram.Set ();
			};
			buts.EnterPressed += () => {
				LcdConsole.WriteLine ("Gyro sensor: " + gyro.ReadAsString());
			};
			terminateProgram.WaitOne ();  
		}
	}
}
