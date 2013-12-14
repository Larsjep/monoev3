using System;

namespace MonoBrickFirmware.Sensors
{
	internal enum ColorRegister : byte
    {
        Version = 0x00, ProductId = 0x08, SensorType = 0x10, Command = 0x41, ColorNumber = 0x42, RedReading = 0x43,
        GreenReading = 0x44, BlueReading = 0x45, RedRawReadingLow = 0x46, RedRawReadingHigh = 0x47, GreenRawReadingLow = 0x48, GreenRawReadingHigh = 0x49,
        BlueRawReadingLow = 0x4A, BlueRawReadingHigh = 0x4B, ColorIndexNo = 0x4c, RedNormalized = 0x4d, GreenNormalized = 0x4e, BlueNormalized = 0x4f
    };
	
	
	/// <summary>
	/// HiTechnic color sensor
	/// </summary>
    public class HiTecColorSensor : I2CSensor, ISensor
    {
        private const byte ColorAddress = 0x02;
        
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.NXT.HiTecColor"/> class.
		/// </summary>
		public HiTecColorSensor(SensorPort port) : base(port, ColorAddress, I2CMode.LowSpeed9V) 
		{ 
			base.Initialise();
		}

		/// <summary>
		/// Returns the color index number (more on http://www.hitechnic.com/)
        /// </summary>
		public int ReadColorIndex()
        {
            return ReadRegister((byte)ColorRegister.ColorNumber, 1)[0]; ;
        }

		/// <summary>
		/// Reads the RGB colors.
		/// </summary>
		/// <returns>
		/// The RGB colors
		/// </returns>
        public RGBColor ReadRGBColor()
        {
            byte[] result = ReadRegister((byte)ColorRegister.RedReading, 3);
            return new RGBColor(result[0], result[1], result[2]);                        
        }

		/// <summary>
		/// Reads the normalized RGB colors
		/// </summary>
		/// <returns>
		/// The normalized RGB colors
		/// </returns>
        public RGBColor ReadNormalizedRGBColor()
        {
            byte[] result = ReadRegister((byte)ColorRegister.RedNormalized, 3);
            return new RGBColor(result[0], result[1], result[2]);
        }

		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
        public string ReadAsString()
        {
            RGBColor color = ReadRGBColor();
            return "Red:" + color.Red + " green:" + color.Green + " blue:" + color.Blue;
        }
    }
}

