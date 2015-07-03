using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Tools;

namespace MonoBrickWebServer.Models
{
	public class LcdModel : ILcdModel
	{
		
		public void SetPixel(int x, int y, bool color)
		{
			Lcd.SetPixel(x, y, color);
			Lcd.Update();				
		}

		public void TakeScreenShot (string directory, string fileName)
		{
			Lcd.TakeScreenShot (directory, fileName);
			Lcd.Update();
		}

		
		public void ClearLines(int y, int count)
		{			
			Lcd.ClearLines(y, count);
			Lcd.Update();
		}
		
		public void Clear()
		{
			Lcd.Clear();
			Lcd.Update();
		}

		public void DrawVLine(int x, int y, int height, bool color)
		{
			Lcd.DrawVLine(new Point(x,y), height, color);
			Lcd.Update(); 
		}

		public void DrawHLine(int x, int y, int length, bool color)
		{
			Lcd.DrawHLine(new Point(x,y), length, color);
			Lcd.Update();
		}


		public void WriteText(int x, int y, string text, bool color)
		{			
			Lcd.WriteText(Font.MediumFont, new Point(x,y), text, color);
			Lcd.Update();
		}
		
		public void WriteTextBox (int xStart, int yStart, int xEnd, int yEnd, string text, bool color, string align)
		{
			Lcd.Alignment lcdAlign;
			if (!Enum.TryParse<Lcd.Alignment> (align, true, out lcdAlign)) 
			{
				lcdAlign = Lcd.Alignment.Center;
			} 
			Lcd.WriteTextBox(Font.MediumFont, new Rectangle(new Point(xStart, yStart), new Point(xEnd, yEnd)), text, color, lcdAlign);
			Lcd.Update();
		}

		public void DrawCircle (int x, int y, ushort radius, bool color, bool fill)
		{	
			Lcd.DrawCircle(new Point(x,y), radius, color, fill);
			Lcd.Update();
		}

		public void DrawEllipse (int x, int y, ushort radiusA, ushort radiusB, bool color, bool fill)
		{
			Lcd.DrawEllipse(new Point(x,y), radiusA, radiusB, color, fill);
			Lcd.Update();	
		}

		public void DrawLine (int xStart, int yStart, int xEnd, int yEnd, bool color)
		{
			Lcd.DrawLine(new Point(xStart,yStart), new Point(xEnd, yEnd), color);
		}

		public void DrawRectangle (int xStart, int yStart, int xEnd, int yEnd,  bool color, bool fill)
		{
			Lcd.DrawRectangle(new Rectangle(new Point(xStart,yStart), new Point(xEnd, yEnd)), color, fill);	
		}
	}
}

