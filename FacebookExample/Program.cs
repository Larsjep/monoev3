using System;
using System.Collections.Generic;
using System.Resources;
using Facebook;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;
using System.Threading;

namespace FacebookExample
{
	//Do this from a terminal before running this program
	//mozroots --import --ask-remove
	class MainClass
	{
		
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent (false);
			string fbToken = "CAACpSl1Qm3cBAHAMyZAY...";//This is not valied
			var fb = new FacebookClient(fbToken);
			Lcd lcd = new Lcd();
			Font f = Font.MediumFont;
			Point offset = new Point(0,25);
			Point p = new Point(10, Lcd.Height-75);
			Point boxSize = new Point(100, 24);
			Rectangle box = new Rectangle(p, p+boxSize);
			var colorSensor = new EV3ColorSensor (SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			LcdConsole.WriteLine("Use color on port1");
			LcdConsole.WriteLine("Enter post value");
			LcdConsole.WriteLine("Esc. terminate");
			buts.EscapePressed += () => { 
				terminateProgram.Set();
			};
			
			buts.EnterPressed += () => { 
				Color color = colorSensor.ReadColor();
				lcd.Clear();
				lcd.WriteTextBox(f, box + offset*0, "Color: " + color, true);
				lcd.WriteTextBox(f, box + offset*1, "Send to Facebook" + color, true);
				lcd.Update();
				colorSensor.ReadColor();
				var me = fb.Get("monobrick.dk") as JsonObject;
				var uid = me["id"];
		    	string url = string.Format("{0}/{1}", uid, "feed");
		    	var argList = new Dictionary<string, object>();
		    	argList["message"] = "A program running MonoBrick Firmware was aked to read the color sensor. Color read: " + colorSensor.ReadColor().ToString();
		    	fb.Post(url, argList);
			};
			terminateProgram.WaitOne (); 
		    		
		}
		
		
	}
}
