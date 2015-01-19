using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Tools;

namespace MonoBrickWebServer.Models
{
	public class LcdModel : ILcdModel
	{
		
		public void SetPixel(int x, int y, bool color)
		{
			Lcd.Instance.SetPixel(x, y, color);
			Lcd.Instance.Update();				
		}

		public void TakeScreenShot (string directory, string fileName)
		{
			Lcd.Instance.TakeScreenShot (directory, fileName);
			Lcd.Instance.Update();
		}

		
		public void ClearLines(int y, int count)
		{			
			Lcd.Instance.ClearLines(y, count);
			Lcd.Instance.Update();
		}
		
		public void Clear()
		{
			Lcd.Instance.Clear();
			Lcd.Instance.Update();
		}

		public void DrawVLine(int x, int y, int height, bool color)
		{
			Lcd.Instance.DrawVLine(new Point(x,y), height, color);
			Lcd.Instance.Update(); 
		}

		public void DrawHLine(int x, int y, int length, bool color)
		{
			Lcd.Instance.DrawHLine(new Point(x,y), length, color);
			Lcd.Instance.Update();
		}


		public void WriteText(int x, int y, string text, bool color)
		{			
			Lcd.Instance.WriteText(Font.MediumFont, new Point(x,y), text, color);
			Lcd.Instance.Update();
		}
		
		public void WriteTextBox (int xStart, int yStart, int xEnd, int yEnd, string text, bool color, string align)
		{
			Lcd.Alignment lcdAlign;
			if (!Enum.TryParse<Lcd.Alignment> (align, true, out lcdAlign)) 
			{
				lcdAlign = Lcd.Alignment.Center;
			} 
			Lcd.Instance.WriteTextBox(Font.MediumFont, new Rectangle(new Point(xStart, yStart), new Point(xEnd, yEnd)), text, color, lcdAlign);
			Lcd.Instance.Update();
		}

		public void DrawCircle (int x, int y, ushort radius, bool color, bool fill)
		{	
			Lcd.Instance.DrawCircle(new Point(x,y), radius, color, fill);
			Lcd.Instance.Update();
		}

		public void DrawEllipse (int x, int y, ushort radiusA, ushort radiusB, bool color, bool fill)
		{
			Lcd.Instance.DrawEllipse(new Point(x,y), radiusA, radiusB, color, fill);
			Lcd.Instance.Update();	
		}

		public void DrawLine (int xStart, int yStart, int xEnd, int yEnd, bool color)
		{
			Lcd.Instance.DrawLine(new Point(xStart,yStart), new Point(xEnd, yEnd), color);
		}

		public void DrawRectangle (int xStart, int yStart, int xEnd, int yEnd,  bool color, bool fill)
		{
			Lcd.Instance.DrawRectangle(new Rectangle(new Point(xStart,yStart), new Point(xEnd, yEnd)), color, fill);	
		}
	}
}

