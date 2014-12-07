using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Tools;

namespace MonoBrickWebServer.Models
{
	public class DummyLcdModel : ILcdModel
	{
		
		public DummyLcdModel ()
		{
		}

		public void SetPixel (int x, int y, bool color)
		{
			Console.WriteLine("Set pixel " + color);
		}

		public void TakeScreenShot (string directory, string fileName)
		{
				//Console.WriteLine("Create screenshot");
				//Code is taken from the firmware code and simply returns a blank LCD
				int Width = 178;
				int Height = 128;
				BmpImage screenshotImage = new BmpImage (24 * 8, (uint)Height, ColorDepth.TrueColor);
				RGB startColor = new RGB (188, 191, 161);
				RGB endColor = new  RGB (219, 225, 206);
				screenshotImage.Clear ();
				float redActual = (float)endColor.Red;
				float greenActual = (float)endColor.Green;
				float blueActual = (float)endColor.Blue;
				int bytesPrLine = ((Width + 31) / 32) * 4;
				float redGradientStep = (float)(endColor.Red - startColor.Red) / Height; 
				float greenGradientStep = (float)(endColor.Green - startColor.Green) / Height; 
				float blueGradientStep = (float)(endColor.Blue - startColor.Blue) / Height;

				RGB color = new RGB ();
				for (int y = Height - 1; y >= 0; y--) {
					for (int x = 0; x < bytesPrLine * 8; x++) {
						color.Red = (byte)redActual;
						color.Green = (byte)greenActual;
						color.Blue = (byte)blueActual;
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

		public void ClearLines (int y, int count)
		{
			Console.WriteLine("Clear lines called with y: " + y + " and count " + count);	
		}
		
		public void Clear ()
		{
			Console.WriteLine("Clear LCD");		
		}

		public void DrawVLine(int xStart, int yStart, int height, bool color)
		{
			Console.WriteLine("Draw V line");		
		}

		public void DrawHLine (int x, int y, int length, bool color)
		{
			Console.WriteLine("Draw H line");
		}

		public void WriteText(int x, int y, string text, bool color)
		{
			Console.WriteLine("Write text to lcd " + text);	
		}
		
		public void WriteTextBox(int xStart, int yStart, int xEnd, int yEnd, string text, bool color, 	string align)
		{
			Console.WriteLine("Write text box to lcd " + text);
		}

		public void DrawCircle (int x, int y, ushort radius, bool color, bool fill)
		{
			Console.WriteLine("Draw circle");
		}

		public void DrawEllipse (int x, int y, ushort radiusA, ushort radiusB, bool color, bool fill)
		{
			Console.WriteLine("Draw ellipse");
		}

		public void DrawLine (int xStart, int yStart, int xEnd, int yEnd, bool color)
		{
			Console.WriteLine("Draw line");
		}

		public void DrawRectangle (int xStart, int yStart, int xEnd, int yEnd, bool color, bool fill)
		{
			Console.WriteLine("Draw rectangle");
		}

	}
}

