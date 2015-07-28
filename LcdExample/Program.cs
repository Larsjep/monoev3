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
		public static void Main (string[] args)
		{
			Bitmap monoLogo = Bitmap.FromResouce(Assembly.GetExecutingAssembly(), "monologo.bitmap");
			EventWaitHandle stopped = new ManualResetEvent(false);
			Lcd.ShowPicture(MonoPicture.Picture);
			Font f = Font.MediumFont;
			Point offset = new Point(0,25);
			Point p = new Point(10, Lcd.Height-75);
			Point boxSize = new Point(100, 24);
			Rectangle box = new Rectangle(p, p+boxSize);
			using (ButtonEvents buts = new ButtonEvents ()) 
			{
				int val = 7;
				buts.EnterPressed += () => { 
					Lcd.Clear ();
					Lcd.WriteTextBox (f, box + offset * 0, "Value = " + val.ToString (), true);
					Lcd.WriteTextBox (f, box + offset * 1, "Hello EV3!!", false);
					Lcd.WriteTextBox (f, box + offset * 2, "Hello World!!", true);	
					Lcd.Update (); 				
					val++;
				};
				buts.UpPressed += () => { 
					Lcd.Clear ();
					Lcd.DrawBitmap (monoLogo, new Point ((int)(Lcd.Width - monoLogo.Width) / 2, 10));	
					Lcd.Update ();	
				};
				buts.DownPressed += () => { 
					Lcd.TakeScreenShot ();
					Lcd.Clear ();
					Lcd.WriteTextBox (f, box + offset * 1, "Screen Shot", true);	
					Lcd.Update ();		
				};
				buts.EscapePressed += () => stopped.Set ();
				stopped.WaitOne ();
			}
			Lcd.WriteTextBox(f, box + offset*0, "Done!", true);
			Lcd.Update();
		}
	}
}
