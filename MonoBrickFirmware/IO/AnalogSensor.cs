using System;

namespace MonoBrickFirmware.IO
{
	/// <summary>
	/// Analog sensor class. Should not be used for inheritance
	/// </summary>
	public sealed class AnalogSensor : AnalogSensorAbstraction
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
		public AnalogSensor (SensorPort port, AnalogMode mode)
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
		public new Int16 ReadPin1AsPct ()
		{
			return base.ReadPin1AsPct();
		}
		
		/// <summary>
		/// Reads pin 6 as percent.
		/// </summary>
		/// <returns>Pin 6 as percent</returns>
		public new Int16 ReadPin6AsPct ()
		{
			return base.ReadPin6AsPct();
		}
		
		
		/// <summary>
		/// Reads Pin 1.
		/// </summary>
		/// <returns>Pin 1 value</returns>
		public new Int16 ReadPin1()
		{
		    return base.ReadPin1();
		}
		
		/// <summary>
		/// Reads pin 5.
		/// </summary>
		/// <returns>Pin5 value</returns>
		public new Int16 ReadPin5()
		{
		    return base.ReadPin5();
		}
		
		/// <summary>
		/// Reads pin 6.
		/// </summary>
		/// <returns>Pin 6 value</returns>
		public new Int16 ReadPin6()
		{
		    return base.ReadPin6(); 
		}
		
		/// <summary>
		/// Reads the value of pin 1 converted to 10 bit
		/// </summary>
		/// <returns>The raw value</returns>
		public new Int16 ReadRaw(){
			return base.ReadRaw();
		}
		
		
	}
}

