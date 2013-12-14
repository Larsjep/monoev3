using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;

namespace HiTechnicI2CExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent (false);
			ButtonEvents buts = new ButtonEvents ();
			HiTecCompass compass = new HiTecCompass(SensorPort.In1);
			HiTecColor colorSensor = new HiTecColor(SensorPort.In2);
			HiTecTilt tilt = new HiTecTilt(SensorPort.In3);
			buts.EscapePressed += () => { 
				terminateProgram.Set ();
			};
			buts.UpPressed += () => { 
				LcdConsole.WriteLine ("Compass sensor: " + compass.ReadAsString());
			};
			buts.EnterPressed += () => {
				LcdConsole.WriteLine ("Color sensor: " + colorSensor.ReadAsString());
				LcdConsole.WriteLine ("Color index: " + colorSensor.ReadColorIndex());
				 
			};
			buts.DownPressed += () => { 
				LcdConsole.WriteLine ("Tilt : " + tilt.ReadAsString());	
			};
			terminateProgram.WaitOne ();  
		}

	}
}
