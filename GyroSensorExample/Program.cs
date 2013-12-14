using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Buttons;
using MonoBrickFirmware.Sensors;

namespace GyroSensorExample.IO
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent (false);
			ButtonEvents buts = new ButtonEvents ();
			var gyro = new HiTecGyro(SensorPort.In1, 600);
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
