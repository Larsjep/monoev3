using System;
using MonoBrickFirmware.Graphics;
namespace ScreenDumpExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			uint width = 20;
			uint height = 20;
			RGB pixel = new RGB();
			BmpImage image = new BmpImage (width, height, ColorDepth.GrayScaleColor);
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					pixel.Red =(byte) j;
					pixel.Green =(byte) j;
					pixel.Blue =(byte) j;
					image.AppendPixel(pixel);
				}
			}
			image.WriteToFile("dump.bmp");
			 
		}
	}
}
