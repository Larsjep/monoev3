using System;
using System.Collections.Generic;
using Gdk;
using GLib;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Tools;
using Gtk;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace EV3MonoBrickSimulator.Stub
{
	internal class LcdStub : EV3Lcd
	{
		private LcdDisplay display;
		private ManualResetEvent lcdUpdate = new ManualResetEvent(false);
		public LcdStub (DrawingArea drawingArea)
		{
			display = new LcdDisplay (drawingArea);
		}


		public override void Update (int yOffset)
		{
			lcdUpdate.Reset ();
			Application.Invoke (
				delegate {
					display.Draw ();
					lcdUpdate.Set();
				}
			);
			lcdUpdate.WaitOne (100);
		}


		public override void SetPixel (int x, int y, bool color)
		{
			base.SetPixel (x, y, color);
			if (color)
			{
				display.SetPixel (x, y);
			} 
			else 
			{
				display.ClearPixel (x, y);
			}
		}


		public override bool IsPixelSet (int x, int y)
		{
			return display.IsPixelSet (x,y);	
		}

		public override void Clear ()
		{
			base.Clear ();
			display.ClearBuffer ();
		}

		public override void ClearLines (int y, int count)
		{
			base.ClearLines (y, count);
			for (int i = 0; i < count; i++)
			{
				for (int x = 0; x < Width; x++)
				{
					display.ClearPixel (x, y + i);
				} 
			} 
		}

		public override void LoadScreen ()
		{
			base.LoadScreen ();
			display.LoadScreen ();
		}

		public override void SaveScreen ()
		{
			base.SaveScreen ();
			display.SaveScreen ();
		}

		public override void DrawBitmap (BitStreamer bs, MonoBrickFirmware.Display.Point p, uint xSize, uint ySize, bool color)
		{
			base.DrawBitmap (bs, p, xSize, ySize, color);
			for (int x = 0; x < xSize; x++) 
			{
				for (int y = 0; y < ySize; y++)
				{
					bool isSet = base.IsPixelSet (x + p.X, y + p.Y);
					if (isSet)
					{
						display.SetPixel (x + p.X, y + p.Y);
					} 
					else 
					{
						display.ClearPixel(x + p.X, y + p.Y);		
					}
				}
			}
		}

		public override void ShowPicture (byte[] picture)
		{
			base.ShowPicture (picture);
			for (int x = 0; x < Width; x++) 
			{
				for (int y = 0; y < Height; y++)
				{
					bool isSet = base.IsPixelSet (x,y);
					if (isSet)
					{
						display.SetPixel (x,y);
					} 
					else 
					{
						display.ClearPixel(x,y);		
					}
				}
			} 
		}

		public override void TakeScreenShot ()
		{
			TakeScreenShot(System.IO.Directory.GetCurrentDirectory(), "ScreenShot-" + string.Format ("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".png");
		}

		public override void TakeScreenShot (string directory, string fileName)
		{
			display.Save (System.IO.Path.Combine (directory, fileName));
		}


		private class LcdDisplay
		{

			private Pixbuf backGroundPixBuffer = new Pixbuf(System.IO.Path.Combine("Images","background.bmp"));
			private Pixbuf lcdBuffer = new Pixbuf(System.IO.Path.Combine("Images","background.bmp"));
			private DrawingArea drawingArea;
			private Pixbuf savedScreenBuffer = new Pixbuf(System.IO.Path.Combine("Images","background.bmp"));
			private Gdk.GC gc;
			private int rowSize;
			private const int Width = 178;
			private const int Height = 128;
			public LcdDisplay (DrawingArea drawingArea)
			{
				this.drawingArea = drawingArea;
				drawingArea.SetSizeRequest (Width, Height);
				gc = new Gdk.GC((Drawable)drawingArea.GdkWindow);
				rowSize = ((backGroundPixBuffer.Width * backGroundPixBuffer.NChannels) + backGroundPixBuffer.Rowstride)/2;
			}

			public void Draw()
			{
					drawingArea.GdkWindow.DrawPixbuf (gc, lcdBuffer, 0, 0, 0, 0, Width, Height, Gdk.RgbDither.Normal, 0, 0);

			}

			public void ClearBuffer()
			{
				lcdBuffer = new Pixbuf (System.IO.Path.Combine("Images","background.bmp"));
			}

			public bool IsPixelSet(int x, int y)
			{
				int index = GetIndex(x, y);
				return (Marshal.ReadInt32( IntPtr.Add (lcdBuffer.Pixels, index)) & 0x00ffffff) == 0x000000;
			}

			public void SetPixel(int x, int y)
			{
				int index = GetIndex(x, y);
				Int32 oldValue = Marshal.ReadInt32( IntPtr.Add (lcdBuffer.Pixels, index));
				Int32 newValue = (int)(oldValue & 0xff000000);
				Marshal.WriteInt32 (IntPtr.Add (lcdBuffer.Pixels, index), newValue);
			}

			public void ClearPixel(int x, int y)
			{
				int index = GetIndex(x, y);
				int backGroundValue = Marshal.ReadInt32( IntPtr.Add (backGroundPixBuffer.Pixels, index));
				int oldValue = Marshal.ReadInt32( IntPtr.Add (lcdBuffer.Pixels, index));
				int newValue = (int)(oldValue & 0xff000000) | (int)(backGroundValue & 0x00ffffff);
				Marshal.WriteInt32 (IntPtr.Add (lcdBuffer.Pixels, index), newValue);

			}

			public void SaveScreen()
			{
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Height; y++)
					{
						int index = GetIndex(x, y);
						int oldValue = Marshal.ReadInt32( IntPtr.Add (lcdBuffer.Pixels, index));
						Marshal.WriteInt32 (IntPtr.Add (savedScreenBuffer.Pixels, index), oldValue);
					} 
				}
			}

			public void LoadScreen()
			{
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Height; y++)
					{
						int index = GetIndex(x, y);
						int oldValue = Marshal.ReadInt32( IntPtr.Add (savedScreenBuffer.Pixels, index));
						Marshal.WriteInt32 (IntPtr.Add (lcdBuffer.Pixels, index), oldValue);
					} 
				}
			}


			public void Save(string fileName)
			{
				lcdBuffer.Save (fileName, "png");
			}

			private int GetIndex(int x, int y)
			{
				return x*backGroundPixBuffer.NChannels + y*rowSize;	
			}
			

		}


	}
}

