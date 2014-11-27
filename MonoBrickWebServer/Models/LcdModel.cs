using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Tools;

namespace MonoBrickWebServer
{
	public class LcdModel
	{
		private bool useDummy;
		public LcdModel (bool useDummy)
		{
			this.useDummy = useDummy;
		}

		public void TakeScreenShot (string directory, string fileName)
		{
			if (!useDummy) 
			{
				Lcd.Instance.TakeScreenShot (directory, fileName);
			} 
			else 
			{
				int Width = 178;
				int Height = 128;
				BmpImage screenshotImage = new BmpImage(24 * 8 , (uint)Height, ColorDepth.TrueColor);
				RGB startColor = new RGB(188,191,161);
				RGB endColor = new  RGB(219,225,206);
				screenshotImage.Clear ();
				float redActual = (float)endColor.Red;
				float greenActual = (float)endColor.Green;
				float blueActual = (float)endColor.Blue;
				int bytesPrLine = ((Width+31)/32)*4;
				float redGradientStep = (float)(endColor.Red - startColor.Red)/Height; 
				float greenGradientStep = (float)(endColor.Green - startColor.Green)/Height; 
				float blueGradientStep = (float)(endColor.Blue - startColor.Blue)/Height;

				RGB color = new RGB ();
				for (int y = Height - 1; y >= 0; y--) {
					for (int x = 0; x < bytesPrLine * 8; x++) 
					{
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
		}
	}
}

