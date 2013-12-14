using System;
using System.Collections.Generic;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.Sensors
{
	
	/// <summary>
	/// Colors that can be read from the EV3 color sensor
	/// </summary>
	public enum Color{ 
		#pragma warning disable 
		None = 0, Black = 1, Blue = 2, Green = 3, 
		Yellow = 4, Red = 5, White = 6, Brown = 7
		#pragma warning restore
	};
	
	/// <summary>
	/// Class that holds RGB colors
	/// </summary>
    public class RGBColor {
        private byte red;
        private byte green;
        private byte blue;
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoBrick.NXT.RGBColor"/> class.
        /// </summary>
        /// <param name='red'>
        /// Red value
        /// </param>
        /// <param name='green'>
        /// Green value
        /// </param>
        /// <param name='blue'>
        /// Blue value
        /// </param>
		public RGBColor(byte red, byte green, byte blue) { this.red = red; this.green = green; this.blue = blue; }
        
		/// <summary>
		/// Gets the red value
		/// </summary>
		/// <value>
		/// The red value
		/// </value>
		public byte Red { get { return red; } }
        
		/// <summary>
		/// Gets the green value
		/// </summary>
		/// <value>
		/// The green value
		/// </value>
		public byte Green { get { return green; } }
        
		/// <summary>
		/// Gets the blue value
		/// </summary>
		/// <value>
		/// The blue value
		/// </value>
		public byte Blue { get { return blue; } }
    }
	
	/// <summary>
	/// Sensor mode when using a NXT or EV3 color sensor
    /// </summary>
    public enum ColorMode { 
		/// <summary>
		/// Use the color sensor to read reflected light
		/// </summary>
		Reflection = AnalogMode.ColorRed, //mode0 for EV3
		
		/// <summary>
		/// Use the color sensor to detect the light intensity
		/// </summary>
		Ambient  = AnalogMode.ColorNone, //mode1 for EV3
		
		/// <summary>
		/// Use the color sensor to distinguish between eight different colors
		/// </summary>
		Color  = AnalogMode.ColorFull, //mode2 for EV3
		
		/// <summary>
		/// Activate the green light on the color sensor. Only works with the NXT Color sensor 
		/// </summary>
		Green = AnalogMode.ColorGreen, // not supported on the EV3
		
		/// <summary>
		/// Activate the green blue on the color sensor. 
		/// </summary>
		Blue = AnalogMode.ColorBlue,//Mode 1 for EV3
	};
	
	public interface IColorSensor{
	
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		string ReadAsString ();
		
		/// <summary>
		/// Read the raw value of the reflected or ambient light. In color mode the color index is returned
		/// </summary>
		/// <returns>The raw value</returns>
		int ReadRaw ();
		
		/// <summary>
		/// Read the intensity of the reflected or ambient light in percent. In color mode the color index is returned
		/// </summary>
		int Read();
		
		/// <summary>
		/// Gets or sets the color mode.
		/// </summary>
		/// <value>The color mode.</value>
		ColorMode Mode{get;set;}
		
		/// <summary>
		/// Reads the color.
		/// </summary>
		/// <returns>The color.</returns>
		Color ReadColor();
	}
	
	
	
	/// <summary>
	/// Class for color sensor that works with both the EV3 and NXT color sensor.
	/// </summary>
	public class ColorSensor : IColorSensor, ISensor{
		private IColorSensor colorSensor;
		private SensorPort port;
		/// <summary>
		/// Initializes a new instance of the NXTColorSensor class in color mode
		/// </summary>
		public ColorSensor (SensorPort port) : this(port, ColorMode.Color)
		{
					
		}
		
		/// <summary>
		/// Initializes a new instance of the NXTColorSensor class.
		/// </summary>
		/// <param name="mode">Mode.</param>
		public ColorSensor (SensorPort port, ColorMode mode)
		{
			Initialize();
			colorSensor.Mode = mode;
			this.port = port;
		}
		
		public void Initialize ()
		{
			ColorMode? mode = null;
			if(colorSensor != null)
				mode = colorSensor.Mode;
				
			if (SensorManager.Instance.GetConnectionType (this.port) == ConnectionType.NXTColor) {
				colorSensor = new NXTColorSensor(this.port);
			
			} 
			else {
				colorSensor = new EV3ColorSensor(this.port);
			}
			if(mode.HasValue)
				colorSensor.Mode = mode.Value;
		}
		
	
		/// <summary>
		/// Gets or sets the color mode.
		/// </summary>
		/// <value>The color mode.</value>
		public ColorMode Mode {
			get{ return colorSensor.Mode;}
			set{ colorSensor.Mode = value;}
		
		}
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public string ReadAsString ()
		{
			return colorSensor.ReadAsString();	
		}
		
		/// <summary>
		/// Read the raw value of the reflected or ambient light. In color mode the color index is returned
		/// </summary>
		/// <returns>The raw value</returns>
		public int ReadRaw ()
		{
			return colorSensor.ReadRaw();	
		}
		
		
		/// <summary>
		/// Read the intensity of the reflected or ambient light in percent. In color mode the color index is returned
		/// </summary>
		public int Read()
		{
			return colorSensor.Read();	
		}
		
		/// <summary>
		/// Reads the color.
		/// </summary>
		/// <returns>The color.</returns>
		public Color ReadColor()
		{
			return colorSensor.ReadColor();	
		}	
	}
	
	public class EV3ColorSensor : UartSensorBase, IColorSensor{
		
		/// <summary>
		/// Initializes a new instance of the NXTColorSensor class in color mode
		/// </summary>
		public EV3ColorSensor (SensorPort port) : this(port, ColorMode.Color)
		{
			
		}
		
		/// <summary>
		/// Initializes a new instance of the NXTColorSensor class.
		/// </summary>
		/// <param name="mode">Mode.</param>
		public EV3ColorSensor (SensorPort port, ColorMode mode) :  base(port)
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
		public string ReadAsString ()
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
		public int Read()
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
			}
			return colorMode;
		}
	}
	
	
	
	public class NXTColorSensor : AnalogSensorBase, IColorSensor{
		
		//Analog memory offsets
    	private const int ColorOffset = 4856;
		private const int ColorBufferSize = 18 * NumberOfSenosrPorts;
		private const int ColorRawOffset = 54;
		
		private const int RedIndex = 0;
    	private const int GreenIndex = 1;
    	private const int BlueIndex = 2;
    	private const int BackgroundIndex = 3;
		
		private const int ADVolts = 3300;
    	private const int ADMax = 2703;
    	private const int MinumumBackGroundValue = (214 / (ADVolts / ADMax));
    	private const int SensorMax = ADMax;
    	private const int SensorResolution = 1023;
		
		private Int16[] colorValues = new Int16[4];
		private Int16[] rawValues = new Int16[4];
		private int[,] calibrationValues = new int[3,4];
    	private int[] calibrationLimits = new int[2];
		
		/// <summary>
		/// Initializes a new instance of the NXTColorSensor class in color mode
		/// </summary>
		public NXTColorSensor (SensorPort port) : this(port, ColorMode.Color)
		{
			
		}
		
		/// <summary>
		/// Initializes a new instance of the NXTColorSensor class.
		/// </summary>
		/// <param name="mode">Mode.</param>
		public NXTColorSensor (SensorPort port, ColorMode mode) :  base(port)
		{
			GetColorData();
			SetMode(AnalogMode);	
		}
		
		/// <summary>
		/// Gets or sets the color mode.
		/// </summary>
		/// <value>The color mode.</value>
		public ColorMode Mode {
			get{return (ColorMode) this.AnalogMode;}
			set{
				//GetColorData();
				SetMode((AnalogMode) value);
			}
		}
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public string ReadAsString ()
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
			int value = 0;
			switch (Mode)
			{
			    case ColorMode.Ambient:
			        value = CalculateRawAverage ();
			        break;
			   	case ColorMode.Color:
			        value = (int)ReadColor();
			        break;
			   	case ColorMode.Reflection:
			        value = CalculateRawAverage ();
			        break;
			   	default:
			   		value = CalculateRawAverage ();
			   		break;
			}
			return value;
		}
		
		protected int CalculateRawAverage ()
		{
			GetRawValues();
			return (int)(rawValues[RedIndex] + rawValues[BlueIndex] + rawValues[GreenIndex])/3;
		}
		
		protected int CalculateRawAverageAsPct ()
		{
			return (CalculateRawAverage () * 100)/ADCResolution;
		}
		
		/// <summary>
		/// Reads the color of the RGB.
		/// </summary>
		/// <returns>The RGB color.</returns>
		public RGBColor ReadRGBColor ()
		{
			GetRawValues();
			Calibrate(colorValues);
			return new RGBColor((byte)rawValues[RedIndex], 	(byte)rawValues[GreenIndex], (byte)rawValues[BlueIndex]);
		}
		
		/// <summary>
		/// Read the intensity of the reflected or ambient light in percent. In color mode the color index is returned
		/// </summary>
		public int Read()
		{
			int value = 0;
			switch (Mode)
			{
			    case ColorMode.Ambient:
			        value = CalculateRawAverageAsPct ();
			        break;
			   	case ColorMode.Color:
			        value = (int)ReadColor();
			        break;
			   	case ColorMode.Reflection:
			        value = CalculateRawAverageAsPct ();
			        break;
			   	default:
			   		value = CalculateRawAverageAsPct ();
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
				color = CalculateColor();
			}
			return color;
		}
		
		
		protected void GetRawValues()
    	{
        	int first = ColorOffset + (int)port * ColorBufferSize + ColorRawOffset;
        	for(int i = 0; i < rawValues.Length; i++)
            	rawValues[i] =  BitConverter.ToInt16(ReadBytes(first + i*2, 2),0); 
        
    	}
    	
    	protected void GetColorData ()
		{
			SetMode (AnalogMode.ColorNone);
			System.Threading.Thread.Sleep (1000);
			int offset = ColorOffset + (int)port * ColorBufferSize;
			for (int i = 0; i < calibrationValues.GetLength(0); i++) {
				for (int j = 0; j < calibrationValues.GetLength(1); j++) {
					calibrationValues [i,j] = BitConverter.ToInt32(ReadBytes(offset, 4),0);
					offset += 4;
				}
			}
	        for(int i = 0; i < calibrationLimits.Length; i++)
	        {
	            calibrationLimits[i] = BitConverter.ToInt16(ReadBytes(offset, 2),0);
	            offset += 2;
	        }
	    }
	
		
		protected Color CalculateColor()
	    {
	        //Taken from the LeJos source code - thanks ;-)
	        GetRawValues();
	        Calibrate(colorValues);
	        int red = colorValues[RedIndex];
	        int blue = colorValues[BlueIndex];
	        int green = colorValues[GreenIndex];
	        int blank = colorValues[BackgroundIndex];
	        // we have calibrated values, now use them to determine the color
	
	        // The following algorithm comes from the 1.29 Lego firmware.
	        if (red > blue && red > green)
	        {
	            // Red dominant color
	            if (red < 65 || (blank < 40 && red < 110))
	                return Color.Black;
	            if (((blue >> 2) + (blue >> 3) + blue < green) &&
	                    ((green << 1) > red))
	                return Color.Yellow;
	            if ((green << 1) - (green >> 2) < red)
	                return Color.Red;
	            if (blue < 70 || green < 70 || (blank < 140 && red < 140))
	                return Color.Black;
	            return Color.White;
	        }
	        else if (green > blue)
	        {
	            // Green dominant color
	            if (green < 40 || (blank < 30 && green < 70))
	                return Color.Black;
	            if ((blue << 1) < red)
	                return Color.Yellow;
	            if ((red + (red >> 2)) < green ||
	                    (blue + (blue>>2)) < green )
	                return Color.Green;
	            if (red < 70 || blue < 70 || (blank < 140 && green < 140))
	                return Color.Black;
	            return Color.White;
	        }
	        else
	        {
	            // Blue dominant color
	            if (blue < 48 || (blank < 25 && blue < 85))
	                return Color.Black;
	            if ((((red*48) >> 5) < blue && ((green*48) >> 5) < blue) ||
	                    ((red*58) >> 5) < blue || ((green*58) >> 5) < blue)
	                return Color.Blue;
	            if (red < 60 || green < 60 || (blank < 110 && blue < 120))
	                return Color.Black;
	            if ((red + (red >> 3)) < blue || (green + (green >> 3)) < blue)
	                return Color.Blue;
	            return Color.White;
	        }
	    }
	    
	    	
		private void Calibrate(Int16[] vals)
    	{
	        // Select the calibration table to use
	        int calTab;
	        int blankVal = rawValues[BackgroundIndex];
	        if (blankVal < calibrationLimits[1])
	            calTab = 2;
	        else if (blankVal < calibrationLimits[0])
	            calTab = 1;
	        else
	            calTab = 0;
	        // Adjust the raw values
	        for (int col = RedIndex; col <= BlueIndex; col++)
	            if (rawValues[col] > blankVal)
	                vals[col] = (Int16) (((rawValues[col] - blankVal) * calibrationValues[calTab,col]) >> 16);
	            else
	                vals[col] = 0;
	        // Adjust the background value
	        if (blankVal > MinumumBackGroundValue)
	            blankVal -= MinumumBackGroundValue;
	        else
	            blankVal = 0;
	        blankVal = (blankVal * 100) / (((SensorMax - MinumumBackGroundValue) * 100) / ADMax);
	        if (blankVal > SensorResolution)
	            blankVal = SensorResolution;
	        vals[BackgroundIndex] = (Int16) ((blankVal * calibrationValues[calTab,BackgroundIndex]) >> 16);
	    }
	}
}

