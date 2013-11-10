using System;
using MonoBrickFirmware.Graphics;
using MonoBrickFirmware.IO;
using System.Reflection;

namespace MonoBrickFirmware.Graphics
{
	static public class LcdConsole
	{		
		private class ConsoleWriter
		{
			Lcd lcd = new Lcd();
			Font f = Font.FromResource(Assembly.GetExecutingAssembly(), "font.profont_7");
			int scrollPos = 0;
			int lines;
			float lineHeigth;
			Rect lineSize;
			public ConsoleWriter()
			{
				lines = (int)(Lcd.Height/f.maxHeight);				
				lineHeigth = (float)Lcd.Height/lines;
				lineSize = new Rect(new Point(0,0), new Point((int)Lcd.Width, (int)f.maxHeight));
			}
				
			public void WriteLine(string line)
			{
				Point p = new Point(0, (int)(scrollPos * lineHeigth));
				lcd.WriteTextBox(f, lineSize + p, line, true);
				scrollPos++;
				
				lcd.Update((int)(scrollPos * lineHeigth));				
				if (scrollPos >= lines)
					scrollPos = 0;
			}
		}
		static ConsoleWriter cw = null;
		static public void WriteLine(string format, params Object[] arg)
		{
			if (cw == null)
				cw = new ConsoleWriter();
			cw.WriteLine(string.Format (format, arg));
		}
	}
}

