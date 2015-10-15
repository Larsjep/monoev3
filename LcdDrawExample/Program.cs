using System;
using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
namespace LcdDraw
{
	public class Program
	{
		public static void Main (string[] args)
		{
			EventWaitHandle stopped = new ManualResetEvent (false); 
			int centerX = Lcd.Width / 2;
			int centerY = Lcd.Height / 2;
			Point center = new Point(centerX,centerY);
			int refreshRate = 100; 
			bool run = true;
			ButtonEvents buts = new ButtonEvents ();  
			buts.EscapePressed += () => {   
				stopped.Set ();
				run = false;  
			}; 

			while (run) {
				for (int k = 0; k < 4; k++) {
					for (int i = 1; i < centerY; i++) {
						Lcd.Clear ();
						Lcd.DrawCircle(center, (ushort)i, true, true);
						Lcd.Update ();
						stopped.WaitOne (refreshRate);
					}
					for (int i = centerY - 1; i > 0; i--) {
						Lcd.Clear ();
						Lcd.DrawCircle(center, (ushort)i, true, true);
						Lcd.Update ();
						stopped.WaitOne (refreshRate);
					}
				}

				for (int k = 0; k < 20; k++) {
					Lcd.Clear();
					Lcd.DrawHLine(center, centerX/2, true);
					stopped.WaitOne (refreshRate);
					Lcd.Clear();
					Lcd.DrawVLine(center, centerY/2, true);
				}

			}
		}
	}
}