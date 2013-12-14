using System;


namespace MonoBrickFirmware.Sensors
{
	
	/// <summary>
	/// Sensor mode when using a sound sensor
    /// </summary>
    public enum SoundMode { 
		/// <summary>
		/// The sound level is measured in A-weighting decibel
		/// </summary>
		SoundDBA = AnalogMode.Set | AnalogMode.Pin5, 
		/// <summary>
		/// The sound level is measured in decibel 
		/// </summary>
		SoundDB = AnalogMode.Set 
	};
    
	/// <summary>
	/// Class for the NXT sound sensor.
	/// </summary>
	public class SoundSensor: AnalogSensorAbstraction, ISensor 
	{
		/// <summary>
		/// Initializes a new instance of the sound sensor class.
		/// </summary>
		public SoundSensor (SensorPort port) : this(port, SoundMode.SoundDB)
		{
			
		}
		
		/// <summary>
		/// Initializes a new instance of the sound sensor class.
		/// </summary>
		/// <param name="mode">Mode.</param>
		public SoundSensor (SensorPort port, SoundMode  mode) :  base(port)
		{
			Mode = mode;
		}
		/// <summary>
		/// Gets or sets the sound mode.
		/// </summary>
		/// <value>The mode.</value>
		public SoundMode Mode {
			get{return (SoundMode) this.AnalogMode;}
			set{SetMode((AnalogMode) value);}
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
			    case SoundMode.SoundDB:
			        s = Read().ToString();
			        break;
			   case SoundMode.SoundDBA:
			        s = Read().ToString();
			        break;
			}
			return s;
		}
		
		/// <summary>
		/// Read the sensor value
		/// </summary>
		public int Read()
		{
			int value = 0;
			switch (Mode)
			{
			    case SoundMode.SoundDB:
			        value =100 - ReadPin1AsPct();
			        break;
			   case SoundMode.SoundDBA:
			        value = 100 -ReadPin1AsPct();
			        break;
			}
			return value;			
		}
		
		/// <summary>
		/// Reads the raw sensor value
		/// </summary>
		/// <returns>The raw value.</returns>
		public int ReadRaw ()
		{
			return 1023 - base.ReadPin1As10Bit();
		}
		
	}
}

