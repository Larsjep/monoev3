using System;
using MonoBrickFirmware.Extensions;
namespace MonoBrickFirmware.Sensors
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
	public class NXTLightSensor : AnalogSensor{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.Sensors.NXTLightSensor"/> class.
		/// </summary>
		/// <param name="port">Port.</param>
		public NXTLightSensor (SensorPort port) : this(port, LightMode.Ambient)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.Sensors.NXTLightSensor"/> class.
		/// </summary>
		/// <param name="port">Port.</param>
		/// <param name="mode">Mode.</param>
		public NXTLightSensor (SensorPort port, LightMode  mode) :  base(port)
		{
			Mode = mode;
		}

		/// <summary>
		/// Gets or sets the light mode.
		/// </summary>
		/// <value>The mode.</value>
		public LightMode Mode {
			get{return (LightMode) this.AnalogMode;}
			set{SetMode((AnalogMode) value);}
		}
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public override string ReadAsString ()
		{
			string s = "";
			switch (AnalogMode)
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
		/// Read the sensor value as percent
		/// </summary>
		public Int16 Read()
		{
			return this.ReadPin1AsPct();
		}
		
		/// <summary>
		/// Reads the raw sensor value.
		/// </summary>
		/// <returns>The raw sensor value.</returns>
		public int ReadRaw ()
		{
			return base.ReadPin1As10Bit();
		}
		
		public override string GetSensorName ()
		{
			return "NXT Light";
		}
		
		public override void SelectNextMode()
		{
			Mode = Mode.Next();
			return;
		}
		
		public override void SelectPreviousMode ()
		{
			Mode = Mode.Next();
			return;
		}
		
		public override int NumberOfModes ()
		{
			return Enum.GetNames(typeof(LightMode)).Length;;
		
		}
        
        public override string SelectedMode ()
		{
			return Mode.ToString();
		}
		
		
	}
}

