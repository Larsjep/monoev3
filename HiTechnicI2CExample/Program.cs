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
			HiTecCompassSensor compass = new HiTecCompassSensor(SensorPort.In1);
			HiTecColorSensor colorSensor = new HiTecColorSensor(SensorPort.In2);
			HiTecTiltSensor tilt = new HiTecTiltSensor(SensorPort.In3);
			LcdConsole.WriteLine("Use compass on port1");
			LcdConsole.WriteLine("Use color on port2");
			LcdConsole.WriteLine("Use tilt on port3");
			LcdConsole.WriteLine("Up read compass");
			LcdConsole.WriteLine("Down read tilt");
			LcdConsole.WriteLine("Enter read color");
			LcdConsole.WriteLine("Esc. terminate");
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
