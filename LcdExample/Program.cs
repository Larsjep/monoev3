using System;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;
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
			
			Lcd lcd = new Lcd();
			lcd.ShowPicture(MonoPicture.Picture);
			Font f = Font.MediumFont;
			Point offset = new Point(0,25);
			Point p = new Point(10, Lcd.Height-75);
			Point boxSize = new Point(100, 24);
			Rect box = new Rect(p, p+boxSize);
			
			ButtonEvents buts = new ButtonEvents();
			int val = 7;
			buts.EnterPressed += () =>
			{ 
				lcd.Clear();
				lcd.WriteTextBox(f, box + offset*0, "Value = " + val.ToString(), true);
				lcd.WriteTextBox(f, box + offset*1, "Hello World!!", false);
				lcd.WriteTextBox(f, box + offset*2, "Hello World!!", true);	
				lcd.Update (); 				
				val++;
			};
			buts.UpPressed += () =>
			{ 
				lcd.Clear();
				lcd.DrawBitmap(monoLogo, new Point((int)(Lcd.Width-monoLogo.Width)/2,0));	
				lcd.Update();	
			};
			buts.DownPressed += () =>
			{ 
				lcd.TakeScreenShot();
				lcd.Clear();
				lcd.WriteTextBox(f, box + offset*1, "Screen Shot", true);	
				lcd.Update();		
			};
			buts.EscapePressed += () => stopped.Set();
			stopped.WaitOne();
			lcd.WriteTextBox(f, box + offset*0, "Done!", true);
			lcd.Update();
		}
	}
}
