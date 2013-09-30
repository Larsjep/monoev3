using System;
using System.Diagnostics;

namespace MonoBrickFirmware
{
	public class BitStreamer
	{
		UInt32[] data;
		int offset;
		int bitOffset;
		const int pixels_pr_word = 32;
		public BitStreamer(UInt32[] data, int offset = 0)
		{
			this.offset = offset;
			this.data = data;
			this.bitOffset = 0;
		}
			
		private void MoveForward(int bits)
		{
			bitOffset += bits;
			if (bitOffset >= pixels_pr_word)
			{
				bitOffset = 0;
				offset++;
			}
		}
		
		public UInt32 GetBits(uint count)
		{
			Debug.Assert(count <= pixels_pr_word);
			int bitsLeft = pixels_pr_word-bitOffset;
			int bitsToTake = Math.Min((int)count, bitsLeft);
			UInt32 result = (data[offset] >> bitOffset) & (0xffffffff >> (pixels_pr_word-bitsToTake));
			MoveForward(bitsToTake);
			if (count > bitsToTake) // Do we need more bits than we got from the first word
			{
				result |= GetBits((uint)(count - bitsToTake)) << bitsToTake;
			}
			return result;
		}
		
		public void PutBits(UInt32 bits, int count)
		{
			int bitsLeft = pixels_pr_word-bitOffset;
			int bitsToPut = Math.Min(count, bitsLeft);
			data[offset] |= bits << bitOffset;				
			MoveForward(bitsToPut);
			if (count > bitsToPut)
			{
				PutBits (bits << bitsToPut, count - bitsToPut);
			}
		}
	}
	
	public class Bitmap
	{
		int width;
		int height;
		UInt32[] data;
		const int pixels_pr_word = 32;
		public Bitmap(int width, int height)
		{
			this.width = width;
			this.height = height;
			data = new UInt32[(width*height+pixels_pr_word-1)/pixels_pr_word];
		}		
	}
}

