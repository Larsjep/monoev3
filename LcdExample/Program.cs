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
			lcd.WriteText(f, 10,Lcd.Height-25, "Hello World!!");
			lcd.Update();
		}
	}
}
