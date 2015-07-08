using System;
using System.Collections.Generic;
using Gdk;
using GLib;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Tools;
using Gtk;
using System.Linq;

namespace MonoBrickFirmwareSimulation.Mock
{
	public class LcdMock : EV3Lcd
	{
		private DrawingArea drawingArea;
	  	private Gdk.GC gc;
		private Dictionary<int, Color> backgroundColors = new Dictionary<int, Color> ();
		private bool backGroundFilled = false;

		public LcdMock (DrawingArea drawingArea)
		{
		  	this.drawingArea = drawingArea;
	      	gc = new Gdk.GC((Drawable)drawingArea.GdkWindow);
		}

		public override void Update (int yOffset)
		{
			Application.Invoke (delegate {
				FillBackGround();
				List<Gdk.Point> pixelSetList = new List<Gdk.Point>();
				List<Gdk.Point> pixelClearList = new List<Gdk.Point>();
				for (int x = 0; x < Width; x++) 
				{
					for (int y = yOffset; y < Height; y++)
					{
						if (IsPixelSet(x, y))
						{
						  pixelSetList.Add(new Gdk.Point(x, y));
						}
						else
						{
							pixelClearList.Add(new Gdk.Point(x, y));  
						}
					}
				}
				for(int y = 0; y < Height; y++)
				{
					var pixelsToDraw = pixelClearList.Where( p => p.Y == y);
					gc.RgbFgColor = backgroundColors[y];
					drawingArea.GdkWindow.DrawPoints(gc, pixelsToDraw.ToArray());
				}
		        gc.RgbFgColor = new Color(0, 0, 0);
				drawingArea.GdkWindow.DrawPoints( new Gdk.GC(drawingArea.GdkWindow), pixelSetList.ToArray());
			});
		}
		private void FillBackGround()
		{
			if (backGroundFilled)
				return;
			backGroundFilled = true;
			float redActual = (float)endColor.Red;
			float greenActual = (float)endColor.Green;
			float blueActual = (float)endColor.Blue;
			List<Gdk.Point> pixel = new List<Gdk.Point>();
			gc.RgbFgColor = new Color(255, 255, 255);

			for (int y = Height - 1; y >= 0; y--) {
				pixel.Clear ();
				Color color = new Color((byte)redActual, (byte)greenActual, (byte)blueActual);
				gc.RgbFgColor = color;
				backgroundColors.Add (y, color);  
				for (int x = 0; x < Width; x++) 
				{
					pixel.Add (new Gdk.Point (x, y));
				}
				drawingArea.GdkWindow.DrawPoints(gc, pixel.ToArray());
				redActual -= redGradientStep;
				greenActual -= greenGradientStep;
				blueActual -= blueGradientStep;
			}
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

