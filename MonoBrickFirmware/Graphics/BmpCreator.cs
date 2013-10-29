using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MonoBrickFirmware.Graphics
{
	
	public enum ColorDepth { TrueColor = 24, LowColor = 8, GrayScaleColor = 9 };
	
	public class RGB{
		public byte Red;
		public byte Green;
		public byte Blue;
	};
		
				
	public class BmpImage : IDisposable{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.Graphics.BmpFile"/> class.
		/// </summary>
		/// <param name="width">Width of image in pixels.</param>
		/// <param name="height">Height of image in pixels.</param>
		/// <param name="colorDepth">Color depth.</param>
		public BmpImage (UInt32 width, UInt32 height, ColorDepth colorDepth)
		{
			stream = new MemoryStream ();
			CreateHeader (width, height, colorDepth);
			appendIndexHorizontal = 0;
		}
		
		/// <summary>
		/// Appends a pixel to the file
		/// </summary>
		/// <param name="pixel">Pixel.</param>
		public void AppendRGB (RGB pixel)
		{
			Write (pixel);
			appendIndexHorizontal++;
			if (appendIndexHorizontal > header.ImageWidth) {
				AddPadding();
				appendIndexHorizontal = 0;
			}
		}
		
		/// <summary>
		/// Clear the pixel content but keep the header
		/// </summary>
		public void Clear ()
		{
			stream.Seek(header.OffsetBits,SeekOrigin.Begin);
		
		}
		
		public void WriteToFile (string path)
		{
			FileStream fs = new FileStream(path, FileMode.Create);
			stream.WriteTo(fs);
			fs.Close();
		}
		
		
		private int appendIndexHorizontal = 0;
		private Header header = new Header();
		private MemoryStream stream;
		private UInt32 paddingSize;
		
		private const int NoCompression = 0x00000;//no compression
		private const int BMPFileType = 0x4d42;
		private const int InfoHeaderSize = 40;
		private const int BMPHeaderSize = 14;
		private const int Planes = 1;
		private const int ColorPaletteSize = 256;//holds 256 RGB color palette = 1024 bytes
		private const int ColorMatrixSize = 6;
		
   		
    	//comments from http://en.wikipedia.org/wiki/BMP_file_format
		//format is little-endian
		private class Header{
			public UInt16 MagicNumber;         	//magic number for file
			public UInt32 FileSize;            	//the size of the BMP file in bytes
			public UInt16 Reserved1;           	//reserved
			public UInt16 Reserved2;           	//reserved
			public UInt32 OffsetBits;          	//the offset, i.e. starting address, of the byte where the bitmap data can be found.
			public UInt32 InfoHeaderSize;      	//the size of this header (40 bytes for windows V3)
			public UInt32 ImageWidth;          	//the bitmap width in pixels (signed integer).
			public UInt32 ImageHeight;         	//the bitmap height in pixels (signed integer)
			public UInt16 Planes;              	//the number of color planes being used. Must be set to 1.
			public ColorDepth ColorDepth;      	//color depth of the image. Typical values are 1, 4, 8, 16, 24 and 32.
			public UInt32 Compression;         	//the compression method being used
			public UInt32 ImageSize;           	//size of the raw bitmap data, and should not be confused with the file size.
			public UInt32 XPixelPerMeter;      	//the horizontal resolution of the image. (pixel per meter, signed integer)
			public UInt32 YPixelPerMeter;      	//the vertical resolution of the image. (pixel per meter, signed integer)
			public UInt32 NumberOfColors;      	//the number of colors in the color palette, or 0 to default to 2n.
			public UInt32 ImportantColors;     	//the number of important colors used, or 0 when every color is important; generally ignored.
			public RGB[] ColorPalette;        //the color palette
		}
	    
	    private Header CreateHeader(UInt32 width, UInt32 height, ColorDepth colorDepth)
		{
			switch(colorDepth){
				case ColorDepth.TrueColor:
					header.MagicNumber = BMPFileType;
					header.FileSize = CalculateFileSize(width,height, colorDepth);
					header.Reserved1 = 0;//not used
					header.Reserved2 = 0;//not used
					header.OffsetBits = BMPHeaderSize + InfoHeaderSize;
					header.InfoHeaderSize = InfoHeaderSize;
					header.ImageWidth = width;
					header.ImageHeight = height;
					header.Planes = Planes;
					header.ColorDepth =  colorDepth;
					header.Compression = NoCompression;
					header.ImageSize = CalculateImageSize(width,height, colorDepth);
					header.XPixelPerMeter = 0; //zero when color depth = 24
					header.YPixelPerMeter = 0; //zero when color depth = 24
					header.NumberOfColors =0; //zero when color depth = 24
					header.ImportantColors =0; //zero when color depth = 24
					paddingSize = (width*3)%4;
					//no need to add color palette
					header.ColorPalette = null;
					Write(header);
					break;
				case ColorDepth.LowColor:
					header.MagicNumber = BMPFileType;
					header.FileSize = CalculateFileSize(width,height, colorDepth);
					header.Reserved1 = 0;//not used
					header.Reserved2 = 0;//not used
					header.OffsetBits = BMPHeaderSize + InfoHeaderSize + ColorPaletteSize * 4;
					header.InfoHeaderSize = InfoHeaderSize;
					header.ImageWidth = width;
					header.ImageHeight = height;
					header.Planes = Planes;
					header.ColorDepth =  colorDepth;
					header.Compression = NoCompression;
					header.ImageSize = CalculateImageSize(width,height, colorDepth);
					header.XPixelPerMeter = 0; //zero when color depth = 8
					header.YPixelPerMeter = 0; //zero when color depth = 8
					header.NumberOfColors =0; //zero when color depth = 8
					header.ImportantColors =0; //zero when color depth = 8
					paddingSize = (width)%4;
					header.ColorPalette = CreateColorPalette();
					Write(header);
					break;
				case ColorDepth.GrayScaleColor:
					header.MagicNumber = BMPFileType;
					header.FileSize = CalculateFileSize(width,height, ColorDepth.LowColor);
					header.Reserved1 = 0;//not used
					header.Reserved2 = 0;//not used
					header.OffsetBits = BMPHeaderSize + InfoHeaderSize + ColorPaletteSize * 4;
					header.InfoHeaderSize = InfoHeaderSize;
					header.ImageWidth = width;
					header.ImageHeight = height;
					header.Planes = Planes;
					header.ColorDepth = ColorDepth.LowColor; //will be set to GrayScaleColor when header has been written
					header.Compression = NoCompression;//no comression
					header.ImageSize = CalculateImageSize(width,height, ColorDepth.LowColor);
					header.XPixelPerMeter = 0; //zero when color depth = 8
					header.YPixelPerMeter = 0; //zero when color depth = 8
					header.NumberOfColors =0; //zero when color depth = 8
					header.ImportantColors =0; //zero when color depth = 8
					paddingSize = (width)%4;
					header.ColorPalette = CreateGrayScalePalette();
					Write(header);
					header.ColorDepth = ColorDepth.GrayScaleColor;
					break;
			}
			return header;
		}
 
	    private RGB[] CreateGrayScalePalette ()
		{
			RGB[] palette = new RGB[ColorPaletteSize];
			int index = 0;
			while (index < ColorPaletteSize)
			{
				palette[index] = new RGB();
				palette[index].Red = (byte)index;
				palette[index].Green = (byte)index;
				palette[index].Blue = (byte)index;
				index++;
			}
			return palette;
	    }
	 
	    private void AddPadding(){
	      	stream.Write(new byte[paddingSize],0,(int) paddingSize);
	    }
			   
	    private UInt32 CalculateImageSize(UInt32 width, UInt32 height, ColorDepth colorDepth){
			uint temp = 0;
			UInt16 colorDepthAsUInt16 = (UInt16) colorDepth;
			if(Convert.ToBoolean(( colorDepthAsUInt16 * width)%32) ){
				temp =(uint) (colorDepthAsUInt16 * width)/32+1;
			}
			else{
			  	temp = (uint) (colorDepthAsUInt16 * width) / 32;
			}
			return 4*temp*height;
	   
	    }
	   
	    private UInt32 CalculateFileSize(UInt32 width, UInt32 height, ColorDepth colorDepth){
			if(colorDepth == ColorDepth.TrueColor)
			{
				return CalculateImageSize(width, height, colorDepth) + BMPHeaderSize + InfoHeaderSize;
			}
			if(colorDepth == ColorDepth.LowColor){
				return CalculateImageSize(width, height, colorDepth) + BMPHeaderSize + InfoHeaderSize + ColorPaletteSize * 4;
			}
			return 0;//default
	    }
	 
	    private void Write (Header header)
		{
			//write bmp_header
			Write (header.MagicNumber);
			Write (header.FileSize);
			Write (header.Reserved1);
			Write (header.Reserved2);
			Write (header.OffsetBits);
			
			//write info header
			Write (header.InfoHeaderSize);
			Write (header.ImageWidth);
			Write (header.ImageHeight);
			Write (header.Planes);
			Write ((UInt16)header.ColorDepth);
			Write (header.Compression);
			Write (header.ImageSize);
			Write (header.XPixelPerMeter);
			Write (header.YPixelPerMeter);
			Write (header.NumberOfColors);
			Write (header.ImportantColors);
			if (header.ColorDepth == ColorDepth.LowColor || header.ColorDepth == ColorDepth.GrayScaleColor) {
				foreach (var rgb in header.ColorPalette) {
					Write (rgb.Blue);
					Write (rgb.Green);
					Write (rgb.Red);
					Write((byte) 0x00);
				}
	      }
	    }
	 	
	 	//Returns the Netscape cube idx of a color
		private byte GetCubeIndex(byte color){
		  byte i= 0;
		  byte j = 0;
		  int result;
		  byte[] colorMatrix = new byte[ColorMatrixSize];
		  colorMatrix[0]=0x00;
		  colorMatrix[1]=0x33;
		  colorMatrix[2]=0x66;
		  colorMatrix[3]=0x99;
		  colorMatrix[4]=0xcc;
		  colorMatrix[5]=0xff;
		  while(i<6){
		    result=color-colorMatrix[i];
		    if(result<0){
		      result=-result;
		    }
		    if(result<=25){
		        j=i;
		        i=10;
		    }
		    i++;
		  }
		  return j;
		}


		//This will return the index of the color in the Netscape color cube
		private byte GetColorIndex(RGB color){
		  return (byte)((GetCubeIndex(color.Red) * 36) + (GetCubeIndex(color.Green) * 6) + GetCubeIndex(color.Blue));
		}

	    private void Write (RGB color)
		{
			byte temp;
			switch(header.ColorDepth){
			    case ColorDepth.TrueColor:
			      Write(color.Blue);
			      Write(color.Green);
			      Write(color.Red);
			      break;
			    case ColorDepth.LowColor:
			      temp= GetColorIndex(color);
			      Write(temp);
			      break;
			    case ColorDepth.GrayScaleColor:
			      temp=color.Red; //red = green = blue
			      Write(temp);
			      break;
  			}
		}
	    
	    private void Write(UInt16 value) {
	      Write(BitConverter.GetBytes(value));  
	    }
	 
	    private void Write(UInt32 value)
	    {
	      Write(BitConverter.GetBytes(value));  
	    }
	 
	    private void Write(byte value) {
	      stream.WriteByte(value);  
	    }
	 
	    private void Write(byte[] data)
	    {
	      stream.Write(data, 0, data.Length);
	    }
	   
		//For 256 color the Netscape Color Cube is used (http://www.webreference.com/dev/graphics/palette.html)
	    private RGB[] CreateColorPalette()
	    {
	      RGB[] palette = null;
	      uint paletteIndex, redIndex, greenIndex, blueIndex;
	      RGB temp = new RGB();
	      byte[] colorMatrix = new byte[ColorMatrixSize];
	      colorMatrix[0] = 0x00;
	      colorMatrix[1] = 0x33;
	      colorMatrix[2] = 0x66;
	      colorMatrix[3] = 0x99;
	      colorMatrix[4] = 0xcc;
	      colorMatrix[5] = 0xff;
	      temp.Red = 0;
	      temp.Green = 0;
	      temp.Blue = 0;
	      palette = new RGB[ColorPaletteSize];
	      paletteIndex = 0;
	      redIndex = 0;
	      while (redIndex < ColorMatrixSize)
	      {
	        temp.Red = colorMatrix[redIndex];
	        greenIndex = 0;
	        while (greenIndex < ColorMatrixSize)
	        {
	          temp.Green = colorMatrix[greenIndex];
	          blueIndex = 0;
	          while (blueIndex < ColorMatrixSize)
	          {
	            temp.Blue = colorMatrix[blueIndex];
	            palette[paletteIndex] = temp;
	            blueIndex++;
	            paletteIndex++;
	          }
	          greenIndex++;
	        }
	        redIndex++;
	      }
	      temp.Red = 0xff;
	      temp.Green = 0xff;
	      temp.Blue = 0xff;
	      while (paletteIndex < ColorPaletteSize)
	      {
	        palette[paletteIndex] = temp;
	        paletteIndex++;
	      }
	      return palette;
	    }
	    
	    public void Dispose()
		{			
			if(stream != null)
				stream.Dispose();
		}
		
		~BmpImage()
		{
			if(stream != null)
				stream.Dispose();
			
		}
	    
  };
}

