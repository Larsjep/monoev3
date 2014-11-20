using System;
using MonoBrickFirmware.Display;

namespace MonoBrickWebServer
{
	public class LcdModel
	{
		public void TakeScreenShot (string directory, string fileName)
		{
			Lcd.Instance.TakeScreenShot(directory,fileName);	
		}
	}
}

