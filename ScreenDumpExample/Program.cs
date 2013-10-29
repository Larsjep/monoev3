using System;
using MonoBrickFirmware.Graphics;
using MonoBrickFirmware.IO;
using System.Reflection;
namespace ScreenDumpExample
{
	class MainClass
	{
		static Bitmap monoLogo = Bitmap.FromResouce(Assembly.GetExecutingAssembly(), "monologo.bitmap");
			
		public static void Main (string[] args)
		{
			Lcd lcd = new Lcd();
			lcd.DrawBitmap(monoLogo, new Point((int)(Lcd.Width-monoLogo.Width)/2,0));	
			lcd.Update();
			lcd.TakeScreenShot();
		}
	}
}
