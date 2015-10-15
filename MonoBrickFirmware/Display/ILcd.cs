using System;

namespace MonoBrickFirmware.Display
{
	public interface ILcd
	{
		int Width { get;}
		int Height { get; }
		void SetPixel(int x, int y, bool color);
		bool IsPixelSet (int x, int y);
		void Update();
		void Update(int yOffset);
		void SaveScreen ();
		void LoadScreen();
		void ShowPicture(byte[] picture);
		void TakeScreenShot ();
		void TakeScreenShot (string directory, string fileName );
		void ClearLines(int y, int count);
		void Clear();
		void DrawVLine(Point startPoint, int height, bool color);
		void DrawHLine(Point startPoint, int length, bool color);
		void DrawBitmap(Bitmap bm, Point p);
		void DrawBitmap(BitStreamer bs, Point p, uint xsize, uint ysize, bool color);
		int TextWidth(Font f, string text);
		void WriteText(Font f, Point p, string text, bool color);
		void DrawArrow (Rectangle r, Lcd.ArrowOrientation orientation, bool color);
		void WriteTextBox(Font f, Rectangle r, string text, bool color);
		void WriteTextBox(Font f, Rectangle r, string text, bool color, Lcd.Alignment aln);
		void DrawCircle (Point center, ushort radius, bool color, bool fill);
		void DrawEllipse (Point center, ushort radiusA, ushort radiusB, bool color, bool fill);
		void DrawLine (Point start, Point end, bool color);
		void DrawRectangle (Rectangle r, bool color, bool fill);
	}
}

