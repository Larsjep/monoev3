using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;

namespace GyroSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent (false);
			GyroMode[] modes = {GyroMode.Angle, GyroMode.AngularVelocity};
			int modeIdx = 0;
			ButtonEvents buts = new ButtonEvents ();
			var gyro = new EV3GyroSensor(SensorPort.In2, GyroMode.Angle);
			LcdConsole.WriteLine("Use gyro on port 1");
			LcdConsole.WriteLine("Up read value");
			LcdConsole.WriteLine("Down rotation count");
			LcdConsole.WriteLine("Left reset");
			LcdConsole.WriteLine("Enter change mode");
			LcdConsole.WriteLine("Esc. terminate");
			buts.EscapePressed += () => { 
				terminateProgram.Set ();
			};
			buts.UpPressed += () => {
				LcdConsole.WriteLine ("Gyro sensor: " + gyro.ReadAsString());
			};
			buts.EnterPressed += () => { 
				modeIdx = (modeIdx+1)%modes.Length;
				gyro.Mode = modes[modeIdx];
				LcdConsole.WriteLine("Mode: " + modes[modeIdx]);
			};
			buts.DownPressed += () => {
				LcdConsole.WriteLine ("Rotation count: " + gyro.RotationCount());
			};
			buts.LeftPressed += () => {
				LcdConsole.WriteLine ("Reset");
				gyro.Reset();
			};
			terminateProgram.WaitOne ();  
		}
	}
}
