using System;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Graphics;

namespace MonoBrickFirmware.IO
{	
	public class Lcd : Ev3Device
	{
		public const int Width = 178;
		public const int Height = 128;
		const int bytesPrLine = (Width+7)/8;
		const int bufferSize = ((Width+7)/8)*Height;
		const int hwBufferLineSize = 60;
		const int hwBufferSize = hwBufferLineSize*Height;			
		
		byte[] displayBuf = new byte[bufferSize];
		public void SetPixel(int x, int y, bool color)
		{
			int index = (x/8)+y*23;
			int bit = x & 0x7;
			if (color)
				displayBuf[index] |= (byte)(1 << bit);
			else
				displayBuf[index] &= (byte)~(1 << bit);
					
		}	
		
		byte[] hwBuffer = new byte[hwBufferSize];
		
		public Lcd() : base("/dev/fb0", hwBufferSize, 0)
		{
		
		}
		
		static byte[] convert = 
		    {
		    0x00, // 000 00000000
		    0xE0, // 001 11100000
		    0x1C, // 010 00011100
		    0xFC, // 011 11111100
		    0x03, // 100 00000011
		    0xE3, // 101 11100011
		    0x1F, // 110 00011111
		    0xFF // 111 11111111
		    }; 
		
		public void Update()
		{
			int inOffset = 0;
		    int outOffset = 0;
		    for(int row = 0; row < Height; row++)
		    {
		        int pixels;
		        for(int i = 0; i < 7; i++)
		        {
		            pixels = displayBuf[inOffset++] & 0xff;
		            pixels |= (displayBuf[inOffset++] & 0xff) << 8;
		            pixels |= (displayBuf[inOffset++] & 0xff) << 16;
		            hwBuffer[outOffset++] = convert[pixels & 0x7];
		            pixels >>= 3;
		            hwBuffer[outOffset++] = convert[pixels & 0x7];
		            pixels >>= 3;
		            hwBuffer[outOffset++] = convert[pixels & 0x7];
		            pixels >>= 3;
		            hwBuffer[outOffset++] = convert[pixels & 0x7];
		            pixels >>= 3;
		            hwBuffer[outOffset++] = convert[pixels & 0x7];
		            pixels >>= 3;
		            hwBuffer[outOffset++] = convert[pixels & 0x7];
		            pixels >>= 3;
		            hwBuffer[outOffset++] = convert[pixels & 0x7];
		            pixels >>= 3;
		            hwBuffer[outOffset++] = convert[pixels & 0x7];
		        }   
		        pixels = displayBuf[inOffset++] & 0xff;
		        pixels |= (displayBuf[inOffset++] & 0xff) << 8;
		        hwBuffer[outOffset++] = convert[pixels & 0x7];
		        pixels >>= 3;
		        hwBuffer[outOffset++] = convert[pixels & 0x7];
		        pixels >>= 3;
		        hwBuffer[outOffset++] = convert[pixels & 0x7];
		        pixels >>= 3;
		        hwBuffer[outOffset++] = convert[pixels & 0x7];
		    } 
			this.Memory.Write(0,hwBuffer);
		}
		
		public void ShowPicture(byte[] picture)
		{
			Array.Copy(picture, displayBuf, picture.Length);
			Update();
		}
		
		public void ClearLines(int y, int count)
		{			
			Array.Clear(displayBuf, bytesPrLine*y, count*bytesPrLine);
		}
		
		public void DrawBitmap(BitStreamer bs, int x, int y, uint xsize, uint ysize)
		{
			for (int yPos = y; yPos != y+ysize; yPos++)
			{
				int BufPos = bytesPrLine*yPos+x/8;
				uint xBitsLeft = xsize;
				int xPos = x;
				
				while (xBitsLeft > 0)
				{
					int bitPos = xPos & 0x7;					
					uint bitsToWrite = Math.Min(xBitsLeft, (uint)(8-bitPos));
					displayBuf[BufPos] |= (byte)(bs.GetBits(bitsToWrite) << bitPos);
					xBitsLeft -= bitsToWrite;
					BufPos++;
				}
				
			}
		}
		
		public void WriteText(Font f, int x, int y, string text)
		{			
			foreach(char c in text)
			{
				CharStreamer cs = f.getChar(c);
				DrawBitmap(cs, x, y, cs.width, cs.height);		
				x += (int)cs.width;				
			}
		}
	}
}

