using System;

namespace MonoBrickWebServer.Models
{
	public interface ILcdModel
	{
		void SetPixel(int x, int y, bool color);

		void TakeScreenShot (string directory, string fileName);
		
		void ClearLines(int y, int count);
		
		void Clear();

		void DrawVLine(int x, int y, int height, bool color);

		void DrawHLine(int x, int y, int length, bool color);

		void WriteText(int x, int y, string text, bool color);
		
		void WriteTextBox(int xStart, int yStart, int xEnd, int yEnd, string text, bool color, string align);

		void DrawCircle (int x, int y, ushort radius, bool color, bool fill);

		void DrawEllipse (int x, int y, ushort radiusA, ushort radiusB, bool color, bool fill);

		void DrawLine (int xStart, int yStart, int xEnd, int yEnd, bool color);

		void DrawRectangle (int xStart, int yStart, int xEnd, int yEnd, bool color, bool fill);
	}
}

