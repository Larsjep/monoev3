using System;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Tools;

namespace MonoBrickFirmware.Display
{	
	public class Lcd
	{
		public const int Width = 178;
		public const int Height = 128;
		public enum Alignment { Left, Center, Right };
		
		
		private const int bytesPrLine = ((Width+31)/32)*4;
		private const int bufferSize = bytesPrLine * Height;
		private const int hwBufferLineSize = bytesPrLine;
		private const int hwBufferSize = hwBufferLineSize*Height;			
		private UnixDevice device;
		private MemoryArea memory;
		private byte[] displayBuf = new byte[bufferSize];
		private byte[] savedScreen = new byte[bufferSize];
		
		private BmpImage screenshotImage = new BmpImage(bytesPrLine * 8 , Height, ColorDepth.TrueColor);
		private RGB startColor = new RGB(188,191,161);
		private RGB endColor = new  RGB(219,225,206);
		private float redGradientStep;
		private float greenGradientStep;  
		private float blueGradientStep; 
		private byte[] hwBuffer = new byte[hwBufferSize];
		
		private static readonly Lcd instance = new Lcd();
		
		private bool IsPixelInLcd(Point pixel)
		{
			return (pixel.X >= 0) && (pixel.Y >= 0) && (pixel.X <= Lcd.Width) && (pixel.Y <= Lcd.Height);
		}

		private bool IsPixelInLcd(int x, int y)
		{
			return	(x >= 0) && (y >= 0) && (x <= Lcd.Width) && (y <= Lcd.Height);
		}

		public void SetPixel(int x, int y, bool color)
		{
			if (!IsPixelInLcd (x, y))
				return;

			int index = (x/8)+ y * bytesPrLine;
			int bit = x & 0x7;
			if (color)
				displayBuf[index] |= (byte)(1 << bit);
			else
				displayBuf[index] &= (byte)~(1 << bit);
					
		}
		
		public enum ArrowOrientation{Left, Right, Down, Up}
		
		
		public bool IsPixelSet (int x, int y)
		{
			int index = (x / 8) + y * bytesPrLine;
			int bit = x & 0x7;
			return (displayBuf[index] & (1 << bit)) != 0;
		}
		
		public static Lcd Instance
		{
			get 
	      	{
				return instance; 
	      	}	
		}
		
		private Lcd()
		{
			device = new UnixDevice("/dev/fb0");
			memory =  device.MMap(hwBufferSize, 0);
			Clear();
			Update();
			
			redGradientStep = (float)(endColor.Red - startColor.Red)/Height; 
			greenGradientStep = (float)(endColor.Green - startColor.Green)/Height; 
			blueGradientStep = (float)(endColor.Blue - startColor.Blue)/Height; 
		}
		
		public void Update(int yOffset = 0)
		{
			int byteOffset = (yOffset % Lcd.Height)*bytesPrLine;
			Array.Copy(displayBuf, byteOffset, hwBuffer, 0, hwBufferSize-byteOffset);
			Array.Copy(displayBuf, 0, hwBuffer, hwBufferSize-byteOffset, byteOffset);			
		    memory.Write(0,hwBuffer);			
		}
		
		public void SaveScreen ()
		{
			Array.Copy(displayBuf,savedScreen, bufferSize);
		}
		
		public void LoadScreen()
		{
			Array.Copy(savedScreen,displayBuf,bufferSize);
		}
		
		public void ShowPicture(byte[] picture)
		{
			Array.Copy(picture, displayBuf, picture.Length);
			Update();
		}


		/// <summary>
		/// Takes the screen shot.
		/// </summary>
		/// <param name="directory">Directory.</param>
		/// <param name="fileName">File name.</param>
		public void TakeScreenShot (string directory, string fileName )
		{
			screenshotImage.Clear ();
			float redActual = (float)endColor.Red;
			float greenActual = (float)endColor.Green;
			float blueActual = (float)endColor.Blue;
			
			RGB color = new RGB ();
			for (int y = Height - 1; y >= 0; y--) {
				for (int x = 0; x < bytesPrLine * 8; x++) {
					if (IsPixelSet (x, y)) {
						color.Blue = 0x00;
						color.Green = 0x00;
						color.Red = 0x00;	
					} else {
						color.Red = (byte)redActual;
						color.Green = (byte)greenActual;
						color.Blue = (byte)blueActual;
					}
					screenshotImage.AppendRGB (color);
				}
				redActual -= redGradientStep;
				greenActual -= greenGradientStep;
				blueActual -= blueGradientStep;
			}
			if (fileName == "") {
				fileName = "ScreenShot" + string.Format ("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".bmp";
			}
			if (!fileName.ToLower ().EndsWith (".bmp")) 
			{
				fileName = fileName + ".bmp";
			}
			screenshotImage.WriteToFile(System.IO.Path.Combine(directory,fileName));
		}
		
		
		public void ClearLines(int y, int count)
		{			
			Array.Clear(displayBuf, bytesPrLine*y, count*bytesPrLine);
		}
		
		public void Clear()
		{
			ClearLines(0, Height);
		}

		public void DrawVLine(Point startPoint, int height, bool color)
		{
			for (var y = 0; y <= height; y++) {
				SetPixel (startPoint.X, startPoint.Y + y, color);			
			}

		}

		public void DrawHLine(Point startPoint, int length, bool color)
		{
			int bytePos = bytesPrLine*startPoint.Y + startPoint.X/8;
			int bitPos = startPoint.X & 0x7;
			int bitsInFirstByte = Math.Min(8 - bitPos, length);			
			byte bitMask = (byte)((0xff >> (8-bitsInFirstByte)) << bitPos);
			
			// Set/clear bits in first byte
			if (color)
				displayBuf[bytePos] |= bitMask;
			else
				displayBuf[bytePos] &= (byte)~bitMask;
			length -= bitsInFirstByte;
			bytePos++;
			while (length >= 8) // Set/Clear all byte full bytes
			{
				displayBuf[bytePos] = color ? (byte)0xff : (byte)0;
				bytePos++;
				length -= 8;
			}
			// Set/clear bits in last byte
			if (length > 0)
			{
				bitMask = (byte)(0xff >> (8-length));
				if (color)
					displayBuf[bytePos] |= bitMask;
				else
					displayBuf[bytePos] &= (byte)~bitMask;				
			}
		}

		public void DrawBitmap(Bitmap bm, Point p)
		{
			DrawBitmap(bm.GetStream(), p, bm.Width, bm.Height, true);
		}

		public void DrawBitmap(BitStreamer bs, Point p, uint xsize, uint ysize, bool color)
		{
			for (int yPos = p.Y; yPos != p.Y+ysize; yPos++)
			{
				int BufPos = bytesPrLine*yPos+p.X/8;
				uint xBitsLeft = xsize;
				int xPos = p.X;
				
				while (xBitsLeft > 0)
				{
					int bitPos = xPos & 0x7;					
					uint bitsToWrite = Math.Min(xBitsLeft, (uint)(8-bitPos));
					if (color)
						displayBuf[BufPos] |= (byte)(bs.GetBits(bitsToWrite) << bitPos);
					else
						displayBuf[BufPos] &= (byte)~(bs.GetBits(bitsToWrite) << bitPos);
					xBitsLeft -= bitsToWrite;
					xPos += (int)bitsToWrite;
					BufPos++;
				}				
			}
		}				

		public int TextWidth(Font f, string text)
		{
			int width = 0;
			foreach(char c in text)
			{
				CharStreamer cs = f.getChar(c);				
				width += (int)cs.width;				
			}
			return width;
		}

		
		public void WriteText(Font f, Point p, string text, bool color)
		{			
			foreach(char c in text)
			{
				CharStreamer cs = f.getChar(c);
				if (p.X+cs.width > Lcd.Width)
					break;
				DrawBitmap(cs, p, cs.width, cs.height, color);		
				p.X += (int)cs.width;				
			}
		}
		
		public void DrawArrow (Rectangle r, ArrowOrientation orientation, bool color)
		{
			int height = r.P2.Y - r.P1.Y;
			int width = r.P2.X - r.P1.X;
			float inc = 0;
			if (orientation == ArrowOrientation.Left || orientation == ArrowOrientation.Right) 
			{
				inc = (((float)height) / 2.0f) / ((float)width); 
			} 
			else 
			{
				inc = (((float)width) / 2.0f) / ((float)height); 
			}
			if (orientation == ArrowOrientation.Left) {
				for (int i = 0; i < width; i++) {
					SetPixel ((int)(r.P1.X + i), (int)(r.P1.Y + height/2), color);
					int points = (int)(inc*(float)i)+1;
					for (int j = 0; j < points; j++) {
						SetPixel ((int)(r.P1.X + i), (int)(r.P1.Y + height / 2 +j), color);
						SetPixel ((int)(r.P1.X + i), (int)(r.P1.Y + height / 2 -j), color);
					}
				}	
			}
			if (orientation == ArrowOrientation.Right) {
				for (int i = 0; i < width; i++) {
					SetPixel ((int)(r.P2.X - i), (int)(r.P1.Y + height/2), color);
					int points = (int)(inc*(float)i)+1;
					for (int j = 0; j < points; j++) {
						SetPixel ((int)(r.P2.X -i), (int)(r.P1.Y + height / 2 +j), color);
						SetPixel ((int)(r.P2.X -i), (int)(r.P1.Y + height / 2 -j), color);
					}
				}	
			}
			if (orientation == ArrowOrientation.Up) {
				for (int i = 0; i < height; i++) {
					
					SetPixel ((int)(r.P1.X + width/2), (int)(r.P1.Y + i), color);
					int points = (int)(inc*(float)i)+1;
					for (int j = 0; j < points; j++) {
						SetPixel ((int)(r.P1.X + width/2+j), (int)(r.P1.Y + i), color);
						SetPixel ((int)(r.P1.X + width/2-j), (int)(r.P1.Y + i), color);
					}
				}	
			}
			if (orientation == ArrowOrientation.Down) {
				for (int i = 0; i < height; i++) {
					
					SetPixel ((int)(r.P1.X + width/2), (int)(r.P2.Y -i), color);
					int points = (int)(inc*(float)i)+1;
					for (int j = 0; j < points; j++) {
						SetPixel ((int)(r.P1.X + width/2+j), (int)(r.P2.Y - i), color);
						SetPixel ((int)(r.P1.X + width/2-j), (int)(r.P2.Y - i), color);
					}
				}	
			}
		}
		
		public void WriteTextBox(Font f, Rectangle r, string text, bool color)
		{
			WriteTextBox(f, r, text, color, Alignment.Left);
		}
		
		public void WriteTextBox(Font f, Rectangle r, string text, bool color, Alignment aln)
		{
			DrawRectangle(r,!color, true);// Clear background
			int xpos = 0;
			if (aln == Alignment.Left)
			{
			} 
			else if (aln == Alignment.Center)
			{
				int width = TextWidth(f, text);
				xpos = (r.P2.X-r.P1.X)/2-width/2;
				if (xpos < 0) xpos = 0;
			}
			else 
			{
				int width = TextWidth(f, text);
				xpos = (r.P2.X-r.P1.X)-width;
				if (xpos < 0) xpos = 0;
			}
			WriteText(f, r.P1+new Point(xpos, 0) , text, color);
		}

		public void DrawCircle (Point center, ushort radius, bool color, bool fill)
		{	
			if (fill) 
			{
				for (int y = -radius; y <= radius; y++) {
					for (int x = -radius; x <= radius; x++) {
						if (x * x + y * y <= radius * radius) {
							SetPixel (center.X + x, center.Y + y, color);
						}
					}
				}	
			} 
			else 
			{
				int f = 1 - radius;
				int ddF_x = 0;
				int ddF_y = -2 * radius;
				int x = 0;
				int y = radius;

				var right = new Point (center.X + radius, center.Y);
				var top = new Point (center.X, center.Y - radius);
				var left = new Point (center.X - radius, center.Y);
				var bottom = new Point (center.X, center.Y + radius);

				SetPixel (right.X, right.Y, color);
				SetPixel (top.X, top.Y, color);
				SetPixel (left.X, left.Y, color);
				SetPixel (bottom.X, bottom.Y, color);

				while (x < y) {
					if (f >= 0) {
						y--;
						ddF_y += 2;
						f += ddF_y;
					}
					x++;
					ddF_x += 2;
					f += ddF_x + 1;

					SetPixel (center.X + x, center.Y + y, color);
					SetPixel (center.X - x, center.Y + y, color);
					SetPixel (center.X + x, center.Y - y, color);
					SetPixel (center.X - x, center.Y - y, color);
					SetPixel (center.X + y, center.Y + x, color);
					SetPixel (center.X - y, center.Y + x, color);
					SetPixel (center.X + y, center.Y - x, color);
					SetPixel (center.X - y, center.Y - x, color);
				}
			}
		}

		public void DrawEllipse (Point center, ushort radiusA, ushort radiusB, bool color, bool fill)
		{

			if (fill) 
			{
				int hh = radiusB * radiusB;
				int ww = radiusA * radiusA;
				int hhww = hh * ww;
				int x0 = radiusA;
				int dx = 0;

				// do the horizontal diameter
				for (int x = -radiusA; x <= radiusA; x++)
					SetPixel(center.X + x, center.Y, color);

				// now do both halves at the same time, away from the diameter
				for (int y = 1; y <= radiusB; y++)
				{
					int x1 = x0 - (dx - 1);  // try slopes of dx - 1 or more

					for (; x1 > 0; x1--)
						if (x1 * x1 * hh + y * y * ww <= hhww)
							break;

					dx = x0 - x1;  // current approximation of the slope
					x0 = x1;

					for (int x = -x0; x <= x0; x++) {
						SetPixel(center.X + x, center.Y - y, color);
						SetPixel(center.X + x, center.Y + y, color);
					}
				}
			} 
			else 
			{
				int dx = 0;
				int dy = radiusB;
				int a2 = radiusA * radiusA;
				int b2 = radiusB * radiusB;
				int err = b2 - (2 * radiusB - 1) * a2;
				int e2;

				do {
					SetPixel (center.X + dx, center.Y + dy, color); /* I. Quadrant */
					SetPixel (center.X - dx, center.Y + dy, color); /* II. Quadrant */
					SetPixel (center.X - dx, center.Y - dy, color); /* III. Quadrant */
					SetPixel (center.X + dx, center.Y - dy, color); /* IV. Quadrant */

					e2 = 2 * err;

					if (e2 < (2 * dx + 1) * b2) {
						dx++;
						err += (2 * dx + 1) * b2;
					}

					if (e2 > -(2 * dy - 1) * a2) {
						dy--;
						err -= (2 * dy - 1) * a2;
					}
				} while (dy >= 0);

				while (dx++ < radiusA) {
					SetPixel (center.X + dx, center.Y, color); 
					SetPixel (center.X - dx, center.Y, color);
				}
			}
		}

		public void DrawLine (Point start, Point end, bool color)
		{
			int height = Math.Abs (end.Y - start.Y);
			int width = Math.Abs (end.X - start.X);

			int ix = start.X;
			int iy = start.Y;
			int sx = start.X < end.X ? 1 : -1;
			int sy = start.Y < end.Y ? 1 : -1;

			int err = width + (-height);
			int e2;

			do {
				SetPixel (ix, iy, color);

				if (ix == end.X && iy == end.Y)
					break;

				e2 = 2 * err;
				if (e2 > (-height)) {
					err += (-height);
					ix += sx;
				}
				if (e2 < width) {
					err += width;
					iy += sy;
				}

			} while (true);
		}

		public void DrawRectangle (Rectangle r, bool color, bool fill)
		{
			if (fill) 
			{
				int length = r.P2.X - r.P1.X;
				for (int y = r.P1.Y; y <= r.P2.Y; ++y)
					DrawHLine(new Point(r.P1.X, y), length, color);
			} 
			else 
			{
				int length = r.P2.X - r.P1.X;
				int height = r.P2.Y - r.P1.Y;

				DrawHLine (new Point (r.P1.X, r.P1.Y), length, color);
				DrawHLine (new Point (r.P1.X, r.P2.Y), length, color);

				DrawVLine (new Point (r.P1.X, r.P1.Y + 1), height - 2, color);
				DrawVLine (new Point (r.P2.X, r.P1.Y + 1), height - 2, color);
			}
		}
			
	}
}

