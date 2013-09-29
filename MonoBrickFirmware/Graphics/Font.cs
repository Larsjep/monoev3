using System;

namespace MonoBrickFirmware.Graphics
{
	public class CharStreamer : BitStreamer
	{
		public uint width;
		public uint height;
		public CharStreamer(uint height, UInt32[] data, int offset) : base(data, offset)
		{
			this.height = height;
			this.width = GetBits(8);
		}
	}
	
	public class Font
	{
		public uint maxWidth;
		public uint maxHeight;
		uint firstChar;
		uint charWordSize;
		UInt32[] data;
		
		public Font (UInt32[] data)
		{
			if (data[0] != 0x544E4F46)
				throw new ArgumentException("Invalid value in font data");
			maxWidth = data[1];
			maxHeight = data[2];
			charWordSize = data[3];
			firstChar = 3;			
			this.data = data;
		}
		
		public CharStreamer getChar(char c)
		{
			CharStreamer result = new CharStreamer(maxHeight, data, (int)(firstChar+(int)c*charWordSize));			
			return result;
		}		
	}
}

