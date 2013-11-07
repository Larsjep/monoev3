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
			buts.EscapePressed += () => { 
				terminateProgram.Set ();
			};
			buts.UpPressed += () => { 
				Console.WriteLine ("Compass sensor: " + compass.ReadAsString());
			};
			buts.DownPressed += () => { 
			};
			buts.RightPressed += () => { 
			
			};
			terminateProgram.WaitOne ();  
		}

	}
}
