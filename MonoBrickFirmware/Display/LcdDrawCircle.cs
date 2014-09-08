using System;

namespace MonoBrickFirmware.Display
{
	public static class DrawCircleLcdExtension
	{

		public static void DrawCircle (this Lcd lcd, Point center, ushort radius, bool color)
		{	
			int f = 1 - radius;
			int ddF_x = 0;
			int ddF_y = -2 * radius;
			int x = 0;
			int y = radius;

			var right = new Point(center.X + radius, center.Y);
			var top = new Point (center.X, center.Y - radius);
			var left = new Point (center.X - radius, center.Y);
			var bottom = new Point(center.X, center.Y + radius);

			lcd.SetPixel (right.X, right.Y, color);
			lcd.SetPixel (top.X, top.Y, color);
			lcd.SetPixel (left.X, left.Y, color);
			lcd.SetPixel (bottom.X, bottom.Y, color);

			while (x < y) {
				if (f >= 0) {
					y--;
					ddF_y += 2;
					f += ddF_y;
				}
				x++;
				ddF_x += 2;
				f += ddF_x + 1;

				lcd.SetPixel (center.X + x, center.Y + y, color);
				lcd.SetPixel (center.X - x, center.Y + y, color);
				lcd.SetPixel (center.X + x, center.Y - y, color);
				lcd.SetPixel (center.X - x, center.Y - y, color);
				lcd.SetPixel (center.X + y, center.Y + x, color);
				lcd.SetPixel (center.X - y, center.Y + x, color);
				lcd.SetPixel (center.X + y, center.Y - x, color);
				lcd.SetPixel (center.X - y, center.Y - x, color);
			}
		}

		public static void DrawCircleFilled (this Lcd lcd, Point center, ushort radius, bool color)
		{
			for (int y = -radius; y <= radius; y++) {
				for (int x = -radius; x <= radius; x++) {
					if (x * x + y * y <= radius * radius) {
						lcd.SetPixel (center.X + x, center.Y + y, color);
					}
				}
			}
		}

	}
}
