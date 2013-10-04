using System;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;
using System.Resources;

namespace example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			Lcd lcd = new Lcd();
			lcd.ShowPicture(MonoPicture.Picture);
			Font f = Font.FromResource(System.Reflection.Assembly.GetExecutingAssembly(), "font.info56_12");
			Point offset = new Point(0,25);
			Point p = new Point(10, Lcd.Height-75);
			Point boxSize = new Point(100, 24);
			Rect box = new Rect(p, p+boxSize);
			lcd.WriteTextBox(f, box + offset*0, "Hello World!!", true);
			lcd.WriteTextBox(f, box + offset*1, "Hello World!!", false);
			lcd.WriteTextBox(f, box + offset*2, "Hello World!!", true);
			lcd.Update();
		}
	}
}
