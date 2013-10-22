using System;

namespace MonoBrickFirmware.IO
{
	/// <summary>
	/// Sensor mode when using a NXT light sensor
    /// </summary>
    public enum LightMode { 
		/// <summary>
		/// Use the lgith sensor to read reflected light
		/// </summary>
		Relection = AnalogMode.Set | AnalogMode.Pin5, 
		
		/// <summary>
		/// Use the light sensor to detect the light intensity
		/// </summary>
		Ambient  = AnalogMode.Set,
	};
	
	/// <summary>
	/// Class for the NXT light sensor
	/// </summary>
	public class LightSensor : AnalogSensor, ISensor{
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.EV3.LightSensor"/> class.
		/// </summary>
		public LightSensor (SensorPort port) : this(port, LightMode.Ambient)
		{
			
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.EV3.LightSensor"/> class.
		/// </summary>
		/// <param name="mode">Mode.</param>
		public LightSensor (SensorPort port, LightMode  mode) :  base(port)
		{
			Mode = mode;
		}
		/// <summary>
		/// Gets or sets the light mode.
		/// </summary>
		/// <value>The mode.</value>
		public LightMode Mode {
			get{return (LightMode) this.mode;}
			set{SetMode((AnalogMode) value);}
		}
		
		public void Initialize ()
		{
			SetMode(mode);
		}

		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public string ReadAsString ()
		{
			string s = "";
			switch (mode)
			{
			    case (AnalogMode)LightMode.Ambient:
			        s = Read().ToString();
			        break;
			   case (AnalogMode)LightMode.Relection:
			        s = Read().ToString();
			        break;
			}
			return s;
		}
		
		/// <summary>
		/// Read this instance.
		/// </summary>
		public int Read()
		{
			return 0;
			/*int value = 0;
			switch (mode)
			{
			    case (SensorMode)LightMode.Ambient:
			        value = GetRead();
			        break;
			   case (SensorMode)LightMode.Relection:
			        value = GetRead();
			        break;
			}
			return value;*/			
		}
		
		/// <summary>
		/// Reads the raw sensor value.
		/// </summary>
		/// <returns>The raw sensor value.</returns>
		public int ReadRaw ()
		{
			return 0;
		}
		
	}
}

