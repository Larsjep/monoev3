using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Tools;
using System.ComponentModel;

namespace MonoBrickFirmware.HardwareIF
{
#if !Ev3Simu
    #region Real Hardware
    public class LcdHAL
    {
        #region SingletonStuff
        // ensures that only one instance of this class can exist, so everybody writes and reads the same variables
        // http://geekswithblogs.net/BlackRabbitCoder/archive/2010/05/19/c-system.lazylttgt-and-the-singleton-design-pattern.aspx
        // static holder for instance, need to use lambda to construct since constructor private (DotNet4 only)
        private static readonly Lazy<LcdHAL> _instance = new Lazy<LcdHAL>(() => new LcdHAL());

        const int hwBufferLineSize = 60;
        const int hwBufferSize = hwBufferLineSize * Lcd.Height;
        private LcdHAL() // hiding creator
        {
            device = new UnixDevice("/dev/fb0");
            memory = device.MMap(hwBufferSize, 0);
        }

        public static LcdHAL Init { get { return _instance.Value; } }

        #endregion //SingletonStuff

        byte[] hwBuffer = new byte[hwBufferSize];

        private UnixDevice device;
        private MemoryArea memory;

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

        public void Update(byte[] displayBuf, int yoffset = 0)
        {
            const int bytesPrLine = 3 * 7 + 2;
            int inOffset = (yoffset % Lcd.Height) * (bytesPrLine);
            int outOffset = 0;
            for (int row = 0; row < Lcd.Height; row++)
            {
                int pixels;
                for (int i = 0; i < 7; i++)
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
                if (inOffset >= Lcd.Height * bytesPrLine)
                    inOffset = 0;
            }
            memory.Write(0, hwBuffer);
        }

        public void Dispose() { device.Dispose(); }
    }
    #endregion Real Hardware
#else
    #region Simulated Hardware
    public class LcdHAL : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

    #region SingletonStuff
        // ensures that only one instance of this class can exist, so everybody writes and reads the same variables
        // http://geekswithblogs.net/BlackRabbitCoder/archive/2010/05/19/c-system.lazylttgt-and-the-singleton-design-pattern.aspx
        // static holder for instance, need to use lambda to construct since constructor private (DotNet4 only)
        private static readonly Lazy<LcdHAL> _instance = new Lazy<LcdHAL>(() => new LcdHAL());

        private LcdHAL()  // hiding creator
        {
            // init output array CurrentBytes
            int _Size = Lcd.Height * Convert.ToInt32((Lcd.Width +7)/8);
            _CurrentBytes = new byte[_Size];
            for (int i = _CurrentBytes.GetLowerBound(0); i <= _CurrentBytes.GetUpperBound(0); i++) 
            {
                _CurrentBytes[i] = 255;
            }
        } 
                // accessor for instance
        public static LcdHAL Init 
        { get 
        { 
            return _instance.Value; 
        } 
        }
 
    #endregion //SingletonStuff

        byte[] _CurrentBytes;

        public byte[] CurrentBytes { get { return _CurrentBytes; } }


        public void Update(byte[] displayBuf, int yoffset = 0)
        {
            for (int i = displayBuf.GetLowerBound(0); i <= displayBuf.GetUpperBound(0); i++) 
            {
                // Reverse sequence and invert bits
                byte WrongByte = displayBuf[i];
                byte NewValue = 0;
                if (WrongByte == 0)
                {
                    NewValue = 255;
                }
                else
                {
                    int Mask = 128;
                    int NewBit = 1;
                    for (int k = 0; k <= 7; k++)
                    {
                        if ((WrongByte & Mask) > 0)
                        {
                            // nothing to do
                        }
                        else
                        {
                            NewValue = Convert.ToByte(NewValue + NewBit);
                        }
                        Mask = Mask >> 1;
                        NewBit = NewBit << 1;
                    }
                }
                _CurrentBytes[i] = NewValue;
            }
            OnPropertyChanged("CurrentBytes");
        }


        //private string ByteToBinary(byte Value)
        //{
        //    byte Mask = 128;
        //    string s = "";
        //    for (int i = 1; i <= 8; i++)
        //    {
        //        if ((Value & Mask) > 0)
        //        {
        //            s = s + "1";
        //        }
        //        else
        //        {
        //            s = s + "0";
        //        }
        //        Mask = Convert.ToByte(Mask / 2);
        //    }
        //    return s;
        //}


        public void Dispose()
        {
            // tbd
        }


        private void OnPropertyChanged(string propertyName)
        {

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }



    #endregion Simulated Hardware
#endif

}
