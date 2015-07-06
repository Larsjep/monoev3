using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Tools;
using Gtk;
using Cairo;

namespace MonoBrickFirmwareSimulation.Mock
{
	public class LcdMock : EV3Lcd
	{
		private readonly Context ctx;
		private Color notSetColor = new Color (255, 255, 255);
		private Color setColor = new Color (0, 0, 0);

		public LcdMock (DrawingArea drawingArea)
		{
			ctx =  Gdk.CairoHelper.Create(drawingArea.GdkWindow);
		}

		public override void Update (int yOffset)
		{
			Application.Invoke (delegate {
				for (int x = 0; x < Width; x++) 
				{
					for (int y = yOffset; y < Height; y++)
					{
							Color drawColor;
							if (IsPixelSet (x, y)) 
							{	
								drawColor = setColor;	
							} 
							else 
							{
								drawColor = notSetColor;
							}
							PointD p1 = new PointD (x,y);
							PointD p2 = new PointD(x+1,y+1);
							ctx.MoveTo(p1);
							ctx.LineTo(p2);
							//ctx.SetSourceRGB(drawColor.R, drawColor.G, drawColor.B);	
							ctx.SetSourceColor(drawColor);
							ctx.Stroke ();
					}

				}
			});
		}

		private Color RGB2Color(RGB color)
		{
			return new Color(color.Red, color.Green, color.Blue);
		}

		/*public void Update (int yOffset)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = yOffset; x < Height; y++)
				{
					RGB drawColor = pixelBuffer[x,y];
					PointD p1 = new PointD (x,y);
					PointD p2 = new PointD(x+1,y+1);
					ctx.MoveTo(p1);
					ctx.LineTo(p2);
					ctx.SetSourceColor(RGB2Color(drawColor));
					ctx.Stroke ();			
				}
			}
		}*/
	}
}

