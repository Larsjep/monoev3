using System;

namespace MonoBrickFirmware.Display
{
	public static class DrawLineLcdExtension
	{

		public static void DrawLine (this Lcd lcd, Point start, Point end, bool color)
		{
			int height = Math.Abs (end.Y - start.Y);
			int width = Math.Abs (end.X - start.X);

			int ix = start.X;
			int iy = start.Y;
			int sx = start.X < end.X ? 1 : -1;
			int sy = start.Y < end.Y ? 1 : -1;

			int err = width + (-height);
			int e2;

			do {
				lcd.SetPixel (ix, iy, color);

				if (ix == end.X && iy == end.Y)
					break;

				e2 = 2 * err;
				if (e2 > (-height)) {
					err += (-height);
					ix += sx;
				}
				if (e2 < width) {
					err += width;
					iy += sy;
				}

			} while (true);
		}

	}
}
