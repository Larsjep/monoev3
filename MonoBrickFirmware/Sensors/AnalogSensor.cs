using System;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Analog sensor class. Should not be used for inheritance
	/// </summary>
	public sealed class AnalogSensor : AnalogSensorBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.AnalogSensor"/> class in set mode
		/// </summary>
		/// <param name="port">Sensor port to use</param>
		public AnalogSensor (SensorPort port): this(port, AnalogMode.Set)
		{
			
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.AnalogSensor"/> class.
		/// </summary>
		/// <param name="port">Sensor port to use</param>
		/// <param name="mode">Sensor mode to use</param>
		public AnalogSensor (SensorPort port, AnalogMode mode):base(port)
		{
			SetMode(mode);
		
		}
		
		public new void SetMode(AnalogMode mode)
	    {
	        base.SetMode(mode);
	    }
		
		/// <summary>
		/// Reads pin 1 as percent.
		/// </summary>
		/// <returns>Pin 1 as percent</returns>
		public new int ReadPin1AsPct ()
		{
			return base.ReadPin1AsPct();
		}
		
		/// <summary>
		/// Reads pin 6 as percent.
		/// </summary>
		/// <returns>Pin 6 as percent</returns>
		public new int ReadPin6AsPct ()
		{
			return base.ReadPin6AsPct();
		}
		
		
		/// <summary>
		/// Reads Pin 1.
		/// </summary>
		/// <returns>Pin 1 value</returns>
		public new int ReadPin1()
		{
		    return base.ReadPin1();
		}
		
		/// <summary>
		/// Reads pin 5.
		/// </summary>
		/// <returns>Pin5 value</returns>
		public new int ReadPin5()
		{
		    return base.ReadPin5();
		}
		
		/// <summary>
		/// Reads pin 6.
		/// </summary>
		/// <returns>Pin 6 value</returns>
		public new int ReadPin6()
		{
		    return base.ReadPin6(); 
		}
		
		/// <summary>
		/// Reads the value of pin 1 converted to 10 bit
		/// </summary>
		/// <returns>The raw value</returns>
		public int ReadRaw(){
			return base.ReadPin1As10Bit();
		}
		
		
	}
}

