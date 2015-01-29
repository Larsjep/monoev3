using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;
using MonoBrickFirmware.Display.Dialogs;

namespace MindsensorsAngleExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ButtonEvents buts = new ButtonEvents ();
			var tokenSource = new CancellationTokenSource();
      		var token = tokenSource.Token;

			var dialog = new InfoDialog("Attach distance sensor", false);
			dialog.Show();
			var sensor = SensorAttachment.Wait<MSAngleSensor>(token);//wait for sensor to attached on any port
			LcdConsole.WriteLine("Up reset angle");
			LcdConsole.WriteLine("Down read RMP");
			LcdConsole.WriteLine("Enter read angle");
			LcdConsole.WriteLine("Left read raw");
			LcdConsole.WriteLine("Esc. terminate");

			buts.EscapePressed += () => { 
				tokenSource.Cancel();
			};
			buts.EnterPressed += () => {
				LcdConsole.WriteLine ("Angle: " + sensor.ReadAngle().ToString());
			};
			buts.UpPressed += () => { 
				LcdConsole.WriteLine ("Reset angle");
				sensor.ResetAngle();
			};
			buts.DownPressed += () => { 
				LcdConsole.WriteLine ("Read RPM: " +  sensor.ReadRPM().ToString());
			};
			buts.LeftPressed += () => { 
				LcdConsole.WriteLine ("Read raw: " +  sensor.ReadRAW().ToString());
			};
			token.WaitHandle.WaitOne();
		}
	}
}
