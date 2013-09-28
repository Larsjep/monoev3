using System;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			Lcd lcd = new Lcd();
			lcd.ShowPicture(MonoPicture.Picture);
		}
	}
}
