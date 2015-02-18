using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;
using MonoBrickFirmware.Display.Dialogs;

namespace MindsensorsDistanceSensorExample
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
			var sensor = SensorAttachment.Wait<MSDistanceSensor>(token);//wait for sensor to attached on any port
			LcdConsole.WriteLine("Up power on");
			LcdConsole.WriteLine("Down power off");
			LcdConsole.WriteLine("Enter read sensor");
			LcdConsole.WriteLine("Left read voltage");
			LcdConsole.WriteLine("Right read range type");
			LcdConsole.WriteLine("Esc. terminate");

			buts.EscapePressed += () => { 
				tokenSource.Cancel();
			};
			buts.EnterPressed += () => {
				LcdConsole.WriteLine ("Sensor reading: " + sensor.ReadAsString());
			};
			buts.UpPressed += () => { 
				LcdConsole.WriteLine ("Power on");
				sensor.PowerOn();
			};
			buts.DownPressed += () => { 
				LcdConsole.WriteLine ("Power off");
				sensor.PowerOff();
			};
			buts.LeftPressed += () => { 
				LcdConsole.WriteLine ("Voltage: " + sensor.GetVolgage());	
			};
			buts.RightPressed += () => { 
				LcdConsole.WriteLine ("Sensor range is  : " + sensor.GetRange());	
			};
			token.WaitHandle.WaitOne();
		}
	}
}
