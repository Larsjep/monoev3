using System;
using System.Reflection;

namespace MonoBrickFirmware.Display
{
	static public class LcdConsole
	{		
		private class ConsoleWriter
		{
			Font f = Font.SmallFont;
			int scrollPos = 0;
			int lines;
			float lineHeigth;
			Rectangle lineSize;
			public ConsoleWriter()
			{
				Reset();
			}
			
			public void Reset ()
			{
				lines = (int)(Lcd.Height/f.maxHeight);				
				lineHeigth = (float)Lcd.Height/lines;
				lineSize = new Rectangle(new Point(0,0), new Point((int)Lcd.Width, (int)f.maxHeight));

			}
				
			public void WriteLine(string line)
			{
				Point p = new Point(0, (int)(scrollPos * lineHeigth));
				Lcd.Instance.WriteTextBox(f, lineSize + p, line, true);
				scrollPos++;
				
				Lcd.Instance.Update((int)(scrollPos * lineHeigth));				
				if (scrollPos >= lines)
					scrollPos = 0;
			}
			
			public void Clear ()
			{
				Lcd.Instance.Clear();
				Reset();
			}
			
		}
		static ConsoleWriter cw = null;
		static public void WriteLine(string format, params Object[] arg)
		{
			if (cw == null)
				cw = new ConsoleWriter();
		    lock (cw)
		    {
		        cw.WriteLine(string.Format(format, arg));
		    }
		}
		
		static public void Clear ()
		{
			if (cw == null) 
			{
				cw = new ConsoleWriter ();//will do a clear
			} 
			cw.Clear ();
			
		}
	}
}

