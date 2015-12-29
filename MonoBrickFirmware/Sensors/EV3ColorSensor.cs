using System;
using MonoBrickFirmware.Extensions;
using System.IO;

namespace MonoBrickFirmware.Sensors
{
	public class EV3ColorSensor : UartSensor{
        /// <summary>
        /// Initializes a new instance of the EV3ColorSensor class in color mode
        /// </summary>
        /// <param name="pollInteval">refresh rate for pollThread</param>
        public EV3ColorSensor(SensorPort port, int pollInterval = 50) : this(port, ColorMode.Color, pollInterval)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EV3ColorSensor class.
        /// </summary>
        /// <param name="mode">Mode.</param>
        /// <param name="pollInteval">refresh rate for pollThread</param>
        public EV3ColorSensor(SensorPort port, ColorMode mode, int pollInterval = 50) : base(port, pollInterval)
        {
            base.Initialise(base.uartMode);
            Mode = mode;
        }

        /// <summary>
        /// Gets or sets the color mode.
        /// </summary>
        /// <value>The color mode.</value>
        public ColorMode Mode {
			get{return SensorModeToColorMode(base.uartMode);}
			set{
				base.SetMode(ColorModeToSensorMode(value));
			}
		}
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public override string ReadAsString ()
		{
			string s = "";
			switch (Mode)
			{
			    case ColorMode.Ambient:
			        s = Read().ToString();
			        break;
			   case ColorMode.Color:
			        s = ReadColor().ToString();
			        break;
			   case ColorMode.Reflection:
			        s = Read().ToString();
			        break;
			   case ColorMode.Blue:
			        s = Read().ToString();
			        break;
                case ColorMode.RGB:
			        s = ReadRGB().ToString();
			        break;
			   default:
			   		s = Read().ToString();
			   		break;
			}
			return s;
		}
		
		/// <summary>
		/// Read the raw value of the reflected or ambient light. In color mode the color index is returned
		/// </summary>
		/// <returns>The raw value</returns>
		public int ReadRaw ()
		{
			return Read();
		}
		
		
		/// <summary>
		/// Read the intensity of the reflected or ambient light in percent. In color mode the color index is returned
		/// </summary>
		public override int Read()
		{
			int value = 0;
			switch (Mode)
			{
			    case ColorMode.Color:
			        value = (int)ReadColor();
			        break;
			   	default:
			   		value = CalculateRawAverageAsPct();
			   		break;
			}
			return value;
		}
		
		/// <summary>
		/// Reads the color.
		/// </summary>
		/// <returns>The color.</returns>
		public Color ReadColor()
		{
			Color color = Color.None;
			if (Mode == ColorMode.Color) {
				color = (Color) (base.ReadByte());
			}
			return color;
		}
		
		private UInt16 GetUInt16(byte[] data, int offset)
		{
			return (UInt16)(data[offset] + (data[offset+1] << 8));
		}
		
        /// <summary>
        /// Reads the RGB color.
        /// </summary>
        /// <returns>The RGB color.</returns>
        public RGBColor ReadRGB()
        {
            RGBColor rgbColor = null;
            if (uartMode == UARTMode.Mode4)
            {
                byte[] rawBytes = ReadBytes(6);
                var r = GetUInt16(rawBytes, 0);
                var g = GetUInt16(rawBytes, 2);
                var b = GetUInt16(rawBytes, 4);
                rgbColor = new RGBColor(r, g ,b);
            }
            return rgbColor;
        }

		protected int CalculateRawAverageAsPct ()
		{
			return(int) base.ReadByte();
		}
	
		private UARTMode ColorModeToSensorMode (ColorMode mode)
		{
			UARTMode sensorMode = UARTMode.Mode0;
			switch (mode) {
				case ColorMode.Ambient:
					sensorMode = UARTMode.Mode1;
					break;
				case ColorMode.Color:
					sensorMode = UARTMode.Mode2;
					break;
				case ColorMode.Blue:
					sensorMode = UARTMode.Mode1;
					break;
				case ColorMode.Green:
					sensorMode = UARTMode.Mode0;//not supported by the EV3 use relection
					break;
				case ColorMode.Reflection:
					sensorMode = UARTMode.Mode0;
					break;
                case ColorMode.RGB:
                    sensorMode = UARTMode.Mode4;
                    break;
			}
			return sensorMode;
		}
		
		
		private ColorMode SensorModeToColorMode (UARTMode mode)
		{
			ColorMode colorMode = ColorMode.Reflection;
			switch (mode) {
				case UARTMode.Mode1:
					colorMode = ColorMode.Ambient;
					break;
				case UARTMode.Mode2:
					colorMode = ColorMode.Color;
					break;
				case UARTMode.Mode0:
					colorMode = ColorMode.Reflection;
					break;
                case UARTMode.Mode4:
                    colorMode = ColorMode.RGB;
                    break;
			}
			return colorMode;
		}
		
		public override string GetSensorName ()
		{
			return "EV3 Color";
		}
		
		public override void SelectNextMode()
		{
			Mode = Mode.Next();
			if(Mode == ColorMode.Green)
				Mode = Mode.Next();
			return;
		}
		
		public override void SelectPreviousMode ()
		{
			Mode = Mode.Previous();
			if(Mode == ColorMode.Green)
				Mode = Mode.Previous();
			return;
		}
		
		public override int NumberOfModes ()
		{
			return Enum.GetNames(typeof(ColorMode)).Length-1;//Green mode not supported;
		}
        
        public override string SelectedMode ()
		{
			return Mode.ToString();
		}
	}
}

