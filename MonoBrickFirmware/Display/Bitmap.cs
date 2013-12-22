using System;
using System.Diagnostics;

namespace MonoBrickFirmware.Display
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
		static uint MagicCodeBitmap = 0x4D544942;
		public uint Width { get; private set; }
		public uint Height { get; private set; }
		UInt32[] data;
		const int pixels_pr_word = 32;
		int dataOffset;
		public Bitmap(uint width, uint height)
		{
			this.Width = width;
			this.Height = height;
			data = new UInt32[(width*height+pixels_pr_word-1)/pixels_pr_word];
			dataOffset = 0;
		}	
		
		public Bitmap(UInt32[] data)
		{
			if (data[0] != MagicCodeBitmap)
				throw new ArgumentException("Invalid value in bitmap data");
			Width = data[1];
			Height = data[2];
			this.data = data;
			dataOffset = 3;			
		}
		
		public BitStreamer GetStream()
		{
			return new BitStreamer(data, dataOffset);
		}
		
		static public Bitmap FromResouce(System.Reflection.Assembly asm, string resourceName)
		{
			System.IO.Stream s = asm.GetManifestResourceStream(resourceName);
			byte[] bytedata = new byte[s.Length];
			s.Read(bytedata, 0, (int)s.Length);
			UInt32[] data = new UInt32[s.Length/4];
			for (int i = 0; i != s.Length/4; ++i)
				data[i] = BitConverter.ToUInt32(bytedata, i*4);
			return new Bitmap(data);
		}			
			
	}
}


