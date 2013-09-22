using System;
using Lego.EV3;

namespace example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			EV3Lcd lcd = new EV3Lcd();
			lcd.ShowPicture(MonoPicture.Picture);
		}
	}
}
