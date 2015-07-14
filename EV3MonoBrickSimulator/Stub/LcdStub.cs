using System;
using System.Collections.Generic;
using Gdk;
using GLib;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Tools;
using Gtk;
using System.Linq;

namespace EV3MonoBrickSimulator.Stub
{
	public class LcdStub : EV3Lcd
	{
		private DrawingArea drawingArea;
		private Gdk.GC gc;
		private Dictionary<int, Color> backgroundColors = new Dictionary<int, Color> ();
		private bool[,] lastSetValue = null;
		public LcdStub (DrawingArea drawingArea)
		{
			this.drawingArea = drawingArea;
			gc = new Gdk.GC((Drawable)drawingArea.GdkWindow);
			gc.RgbFgColor = new Color(255, 255, 255);
			lastSetValue = new bool[Width,Height];
		}

		public override void Update (int yOffset)
		{
			Application.Invoke (delegate 
				{
					int offSet = yOffset;
					Draw(offSet, false);	
				}
			);
		}

		public void FillBackGround()
		{
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
					lastSetValue [x,y] = false;
				}
				drawingArea.GdkWindow.DrawPoints (gc, pixel.ToArray ());
				redActual -= redGradientStep;
				greenActual -= greenGradientStep;
				blueActual -= blueGradientStep;
			}
			Draw (0, true);
		}

		private void Draw(int yOffset, bool drawBackGround)
		{
			List<Gdk.Point> pixelSetList = new List<Gdk.Point>();
			List<Gdk.Point> pixelClearList = new List<Gdk.Point>();
			for (int x = 0; x < Width; x++) 
			{
				for (int y = yOffset; y < Height; y++)
				{
					if (IsPixelSet(x, y))
					{
						if(!lastSetValue[x,y])
						{
							pixelSetList.Add(new Gdk.Point(x, y));
						}
						lastSetValue [x,y] = true;
					}
					else
					{
						if (lastSetValue [x, y] || drawBackGround) 
						{
							pixelClearList.Add(new Gdk.Point(x, y));	
						}
						lastSetValue[x,y] = false;
					}
				}
			}
			for(int y = 0; y < Height; y++)
			{
				var pixelsToDraw = pixelClearList.Where( p => p.Y == y).ToArray();
				gc.RgbFgColor = backgroundColors[y];
				if(pixelsToDraw.Length != 0)
				{
					drawingArea.GdkWindow.DrawPoints (gc, pixelsToDraw);
				}
			}
			if(pixelSetList.Count != 0)
			{
				gc.RgbFgColor = new Color(0, 0, 0);
				drawingArea.GdkWindow.DrawPoints( new Gdk.GC(drawingArea.GdkWindow), pixelSetList.ToArray());
			}
		}


	}
}

