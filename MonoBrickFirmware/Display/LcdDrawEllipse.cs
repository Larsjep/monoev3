using System;

namespace MonoBrickFirmware.Display
{
	public static class DraeEllipseLcdExtension
	{

		public static void DrawEllipse(this Lcd lcd, Point center, ushort radiusA, ushort radiusB, bool color)
		{

			int dx = 0;
			int dy = radiusB;
			int a2 = radiusA * radiusA;
			int b2 = radiusB * radiusB;
			int err = b2 - (2 * radiusB - 1) * a2;
			int e2;

			do {
				lcd.SetPixel(center.X + dx, center.Y + dy, color); /* I. Quadrant */
				lcd.SetPixel(center.X - dx, center.Y + dy, color); /* II. Quadrant */
				lcd.SetPixel(center.X - dx, center.Y - dy, color); /* III. Quadrant */
				lcd.SetPixel(center.X + dx, center.Y - dy, color); /* IV. Quadrant */

				e2 = 2 * err;

				if (e2 < (2 * dx + 1) * b2)
				{
					dx++;
					err += (2 * dx + 1) * b2;
				}

				if (e2 > -(2 * dy - 1) * a2)
				{
					dy--;
					err -= (2 * dy - 1) * a2;
				}
			} while (dy >= 0);

			while (dx++ < radiusA)
			{
				lcd.SetPixel(center.X + dx, center.Y, color); 
				lcd.SetPixel(center.X - dx, center.Y, color);
			}
		}

		public static void DrawEllipseFilled
		(this Lcd lcd, Point center, ushort radiusA, ushort radiusB, bool color)
		{
			int hh = radiusB * radiusB;
			int ww = radiusA * radiusA;
			int hhww = hh * ww;
			int x0 = radiusA;
			int dx = 0;

			// do the horizontal diameter
			for (int x = -radiusA; x <= radiusA; x++)
				lcd.SetPixel(center.X + x, center.Y, color);

			// now do both halves at the same time, away from the diameter
			for (int y = 1; y <= radiusB; y++)
			{
				int x1 = x0 - (dx - 1);  // try slopes of dx - 1 or more

				for (; x1 > 0; x1--)
					if (x1 * x1 * hh + y * y * ww <= hhww)
						break;

				dx = x0 - x1;  // current approximation of the slope
				x0 = x1;

				for (int x = -x0; x <= x0; x++) {
					lcd.SetPixel(center.X + x, center.Y - y, color);
					lcd.SetPixel(center.X + x, center.Y + y, color);
				}
			}
		}

	}
}
