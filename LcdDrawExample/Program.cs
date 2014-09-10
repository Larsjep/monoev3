using System;
using MonoBrickFirmware.Display;

namespace LcdDraw
{
	public class Program
	{
		public static void Main (string[] args)
		{
			Lcd.Instance.Clear();
			Lcd.Instance.DrawCircle(new Point(Lcd.Width/2,Lcd.Height/2), 100, true);
			Lcd.Instance.Update();
		}
	}
}