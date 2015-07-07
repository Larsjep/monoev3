using System;
using System.Collections.Generic;
using Gdk;
using GLib;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Tools;
using Gtk;
using GC = Gdk.GC;

namespace MonoBrickFirmwareSimulation.Mock
{
	public class LcdMock : EV3Lcd
	{
		private Color notSetColor = new Color (255, 255, 255);
		private Color setColor = new Color (0, 0, 0);
	  private DrawingArea drawingArea;
	  private Gdk.GC gc;
    public LcdMock (DrawingArea drawingArea)
		{
		  this.drawingArea = drawingArea;
      gc = new Gdk.GC((Drawable)drawingArea.GdkWindow);
      /*gc.RgbBgColor = new Gdk.Color(255, 255, 255);
      gc.RgbFgColor = new Gdk.Color(0, 0, 0);
      gc.SetLineAttributes(3, LineStyle.OnOffDash, CapStyle.Projecting, JoinStyle.Round);*/
		}

		public override void Update (int yOffset)
		{
			Application.Invoke (delegate {
        List<Gdk.Point> pixelSetList = new List<Gdk.Point>();
        List<Gdk.Point> pixelClearList = new List<Gdk.Point>();
        for (int x = 0; x < Width; x++) 
				{
					for (int y = yOffset; y < Height; y++)
					{
					  if (IsPixelSet(x, y))
					  {
              pixelSetList.Add(new Gdk.Point(x, y));
              /*gc.RgbBgColor = new Gdk.Color(255, 255, 255);
              gc.RgbFgColor = new Gdk.Color(0, 0, 0);
              drawingArea.GdkWindow.DrawPoint(gc, x, y);*/
					  }
					  else
					  {
              pixelClearList.Add(new Gdk.Point(x, y));
              /*gc.RgbBgColor = new Gdk.Color(0, 0, 0);
              gc.RgbFgColor = new Gdk.Color(255, 255, 255);
              drawingArea.GdkWindow.DrawPoint(gc, x,y);*/
            }
					  //pixelList.Add(new Gdk.Point(x, y));
					}
				}
        gc.RgbBgColor = new Gdk.Color(0, 0, 0);
        gc.RgbFgColor = new Gdk.Color(255, 255, 255);
        drawingArea.GdkWindow.DrawPoints(gc, pixelClearList.ToArray());
        gc.RgbBgColor = new Gdk.Color(255, 255, 255);
        gc.RgbFgColor = new Gdk.Color(0, 0, 0);
        drawingArea.GdkWindow.DrawPoints(gc, pixelSetList.ToArray());
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

