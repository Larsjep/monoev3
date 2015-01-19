using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace ImageConverter
{
    class Program
    {
        static uint MagicCodeBitmap = 0x4D544942;

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

            public UInt32 GetBits(int count)
            {
                Debug.Assert(count <= pixels_pr_word);
                int bitsLeft = pixels_pr_word - bitOffset;
                int bitsToTake = Math.Min(count, bitsLeft);
                UInt32 result = (data[offset] >> bitOffset) & (0xffffffff >> (pixels_pr_word - bitsToTake));
                MoveForward(bitsToTake);
                if (count > bitsToTake) // Do we need more bits than we got from the first word
                {
                    result |= GetBits(count - bitsToTake) << bitsToTake;
                }
                return result;
            }

            public void PutBits(UInt32 bits, int count)
            {
                int bitsLeft = pixels_pr_word - bitOffset;
                int bitsToPut = Math.Min(count, bitsLeft);
                data[offset] |= bits << bitOffset;
                MoveForward(bitsToPut);
                if (count > bitsToPut)
                {
                    PutBits(bits << bitsToPut, count - bitsToPut);
                }
            }
        }

        
        static void convertBitmap(string inputFile, string outputFile)
        {
            Image im = Bitmap.FromFile(inputFile);
            Bitmap b = new Bitmap(im);

            using (FileStream of = new FileStream(outputFile, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(of))
            {
                bw.Write(MagicCodeBitmap);
                bw.Write(b.Width);
                bw.Write(b.Height);

                UInt32[] bitmapData = new UInt32[(b.Width * b.Height + 31) / 32];
                BitStreamer bs = new BitStreamer(bitmapData);

                for (int y = 0; y != b.Height; ++y)
                {
                    for (int x = 0; x != b.Width; ++x)
                    {
                        if (b.GetPixel(x, y).B < 200)
                            bs.PutBits(1, 1);
                        else
                            bs.PutBits(0, 1);
                    }
                }
                foreach (var word in bitmapData)
                    bw.Write(word);
            }

        }

        static int Main(string[] args)
        {
            Console.WriteLine("MonoBrick image converter");
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: ImageConverter inputfile outputfile");
                return -1;
            }
            convertBitmap(args[0], args[1]);
            return 0;
        }
    }
}
