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
			Font f = Font.FromResource(System.Reflection.Assembly.GetExecutingAssembly(), "example.emulogicFont");
			lcd.WriteText(f, 10,10, "Hello");
			lcd.WriteText(f, 10,10+18+2, "World!");
			lcd.Update();
		}
	}
}
