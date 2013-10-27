using System;
using System.Collections.Generic;
using System.Resources;
using Facebook;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace FacebookExample
{
	class MainClass
	{
		
		
		public static void Main (string[] args)
		{
			//var fb = new FacebookClient(args[0]);
			Lcd lcd = new Lcd();
			Font f = Font.FromResource(System.Reflection.Assembly.GetExecutingAssembly(), "font.info56_12");
			Point offset = new Point(0,25);
			Point p = new Point(10, Lcd.Height-75);
			Point boxSize = new Point(100, 24);
			Rect box = new Rect(p, p+boxSize);
			bool run = true;
			var colorSensor = new ColorSensor (SensorPort.In1);
			var touch = new TouchSensor (SensorPort.In4);
			var motor = new Motor (MotorPort.OutC);
			ButtonEvents buts = new ButtonEvents ();
			sbyte speed = 0; 
			buts.EscapePressed += () => { 
				run = false;
			};
			
			buts.EnterPressed += () => { 
				Color color = colorSensor.ReadColor();
				lcd.Clear();
				lcd.WriteTextBox(f, box + offset*0, "Color: " + color, true);
				lcd.WriteTextBox(f, box + offset*1, "Send to Facebook" + color, true);
				lcd.Update();
				/*colorSensor.ReadColor();
				var me = fb.Get("monobrick.dk") as JsonObject;
				var uid = me["id"];
		    	string url = string.Format("{0}/{1}", uid, "feed");
		    	var argList = new Dictionary<string, object>();
		    	argList["message"] = "A program running MonoBrick Firmware was aked to read the color sensor. Color read: " + colorSensor.ReadColor().ToString();
		    	fb.Post(url, argList);*/
			};
			buts.LeftPressed += () => { 
				motor.ResetTacho ();
				motor.On (-50, 360 * 4, true, false);
				lcd.Clear();
				lcd.WriteTextBox(f, box + offset*0, "Tacho: " + motor.GetTachoCount(), true);
				lcd.Update();
				System.Threading.Thread.Sleep(200);
				do{
					lcd.Clear();
					lcd.WriteTextBox(f, box + offset*0, "Tacho: " + motor.GetTachoCount(), true);
					lcd.Update();
					System.Threading.Thread.Sleep(50);
				}while(motor.IsRunning());
				motor.Off ();
				lcd.Clear();
				lcd.WriteTextBox(f, box + offset*0, "Tacho: " + motor.GetTachoCount(), true);
				lcd.Update();
			};
			buts.RightPressed += () => { 
				motor.ResetTacho ();
				motor.On (50, 360 * 4, true, false);
				lcd.Clear();
				lcd.WriteTextBox(f, box + offset*0, "Tacho: " + motor.GetTachoCount(), true);
				lcd.Update();
				System.Threading.Thread.Sleep(200);
				do{
					lcd.Clear();
					lcd.WriteTextBox(f, box + offset*0, "Tacho: " + motor.GetTachoCount(), true);
					lcd.Update();
					System.Threading.Thread.Sleep(50);
				}while(motor.IsRunning());
				motor.Off ();
				lcd.Clear();
				lcd.WriteTextBox(f, box + offset*0, "Tacho: " + motor.GetTachoCount(), true);
				lcd.Update();
					
			};
			buts.UpPressed += () => { 
				if (speed < 100)
					speed = (sbyte)(speed + 10);      
				motor.On (speed);
				lcd.Clear();
				lcd.WriteTextBox(f, box + offset*0, "Speed set to: " + speed, true);
				lcd.Update(); 
			};
			buts.DownPressed += () => { 
				if (speed > -100)
					speed = (sbyte)(speed - 10);      
				motor.On (speed);
				lcd.Clear();
				lcd.WriteTextBox(f, box + offset*0, "Speed set to: " + speed, true);
				lcd.Update();    
			};  
			while (run) {
				System.Threading.Thread.Sleep (50);
				if (touch.IsPressed ()) {
					motor.Brake();
					motor.Off ();
					speed = 0;
					lcd.Clear();
					lcd.WriteTextBox(f, box + offset*0, "Motor was stopped: " + speed, true);
					lcd.Update(); 
				}
			}
		    		
		}
		
		
	}
}
