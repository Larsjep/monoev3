using System;

namespace MonoBrickFirmware.Display
{
	public static class DrawRectangleLcdExtension
	{

		public static void DrawRectangle(this Lcd lcd, Rectangle r, bool color)
		{
			int length = r.P2.X - r.P1.X;
			int height = r.P2.Y - r.P1.Y;

			DrawHLineInLcd(lcd, new Point(r.P1.X, r.P1.Y), length, color);
			DrawHLineInLcd(lcd, new Point(r.P1.X, r.P2.Y), length, color);

			DrawVLineInLcd(lcd, new Point(r.P1.X, r.P1.Y+1), height-2, color);
			DrawVLineInLcd(lcd, new Point(r.P2.X, r.P1.Y+1), height-2, color);
		}

		private static void DrawHLineInLcd(Lcd lcd, Point startPoint, int length, bool color)
		{
			for (var x = 0; x <= length; x++) {
				lcd.SetPixel (startPoint.X + x, startPoint.Y, color);			
			}

		}

		private static void DrawVLineInLcd(Lcd lcd, Point startPoint, int height, bool color)
		{
			for (var y = 0; y <= height; y++) {
				lcd.SetPixel (startPoint.X, startPoint.Y + y, color);			
			}

		}


	}
}
