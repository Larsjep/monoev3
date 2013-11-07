using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

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
			HiTecGyro gyro = new HiTecGyro(SensorPort.In3, 600);
			UltraSonicSensor us = new UltraSonicSensor(SensorPort.In4);
			buts.EscapePressed += () => { 
				terminateProgram.Set ();
			};
			buts.UpPressed += () => { 
				Console.WriteLine ("Compass sensor: " + compass.ReadAsString());
			};
			buts.EnterPressed += () => {
				Console.WriteLine ("Color sensor: " + colorSensor.ReadAsString());
				Console.WriteLine ("Color index: " + colorSensor.ReadColorIndex());
				 
			};
			buts.DownPressed += () => { 
				Console.WriteLine ("Gyro sensor: " + gyro.ReadAsString());
			};
			buts.LeftPressed += () => { 
				Console.WriteLine ("Ultra Sonic sensor: " + us.ReadAsString());	
			};
			terminateProgram.WaitOne ();  
		}

	}
}
