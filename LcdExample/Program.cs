using System;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace example
{
	class MainClass
	{
		static EventWaitHandle stopped = new ManualResetEvent(false);
		static Bitmap monoLogo = Bitmap.FromResouce(Assembly.GetExecutingAssembly(), "monologo.bitmap");
		public static void Main (string[] args)
		{
			 new Motor(MotorPort.OutA).Off();
			
			Lcd.Instance.ShowPicture(MonoPicture.Picture);
			Font f = Font.MediumFont;
			Point offset = new Point(0,25);
			Point p = new Point(10, Lcd.Height-75);
			Point boxSize = new Point(100, 24);
			Rectangle box = new Rectangle(p, p+boxSize);
			
			ButtonEvents buts = new ButtonEvents();
			int val = 7;
			buts.EnterPressed += () =>
			{ 
				Lcd.Instance.Clear();
				Lcd.Instance.WriteTextBox(f, box + offset*0, "Value = " + val.ToString(), true);
				Lcd.Instance.WriteTextBox(f, box + offset*1, "Hello World!!", false);
				Lcd.Instance.WriteTextBox(f, box + offset*2, "Hello World!!", true);	
				Lcd.Instance.Update (); 				
				val++;
			};
			buts.UpPressed += () =>
			{ 
				Lcd.Instance.Clear();
				Lcd.Instance.DrawBitmap(monoLogo, new Point((int)(Lcd.Width-monoLogo.Width)/2,10));	
				Lcd.Instance.Update();	
			};
			buts.DownPressed += () =>
			{ 
				Lcd.Instance.TakeScreenShot();
				Lcd.Instance.Clear();
				Lcd.Instance.WriteTextBox(f, box + offset*1, "Screen Shot", true);	
				Lcd.Instance.Update();		
			};
			buts.EscapePressed += () => stopped.Set();
			stopped.WaitOne();
			Lcd.Instance.WriteTextBox(f, box + offset*0, "Done!", true);
			Lcd.Instance.Update();
		}
	}
}
