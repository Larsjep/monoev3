using System;
using MonoBrickFirmware.Extensions;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Class for the EV3 ultrasonic sensor
	/// </summary>
	public class EV3UltrasonicSensor : UartSensor{
		/// <summary>
		/// Initializes a new instance of the EV3 Ultrasonic Sensor.
		/// </summary>
		public EV3UltrasonicSensor (SensorPort port) : this(port, UltraSonicMode.Centimeter)
		{
			
		}
		
		/// <summary>
		/// Initializes a new instance of the EV3 Ultrasonic Sensor.
		/// </summary>
		/// <param name="mode">Mode.</param>
		public EV3UltrasonicSensor (SensorPort port, UltraSonicMode mode) :  base(port)
		{
			base.Initialise(base.uartMode);
			Mode = mode;
		}
		
		/// <summary>
		/// Gets or sets the Gyro mode. 
		/// </summary>
		/// <value>The mode.</value>
		public UltraSonicMode Mode {
			get{return (UltraSonicMode) base.uartMode;}
			set{SetMode((UARTMode) value);}
		}

		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public override string ReadAsString ()
		{
			string s = "";
			switch ((UltraSonicMode)base.uartMode)
			{
			    case UltraSonicMode.Centimeter:
			        s = Read().ToString() + " cm";
			        break;
			   	case UltraSonicMode.Inch:
			        s = Read().ToString() +  " inch";
			        break;
			    case UltraSonicMode.Listen:
			        s = Read().ToString();
			        break;
			}
			return s;
		}
		
		/// <summary>
		/// Read the sensor value. Result depends on the mode
		/// </summary>
		public int Read ()
		{
			if (Mode == UltraSonicMode.Listen) 
			{
				if(ReadByte() != 0)
					return 1;
				return 0;
			}
			return (int) BitConverter.ToInt16(ReadBytes(2),0);
		}
		
		public override string GetSensorName ()
		{
			return "EV3 Ultrasonic";
		}
		
		public override void SelectNextMode()
		{
			Mode = Mode.Next();
			return;
		}
		
		public override void SelectPreviousMode ()
		{
			Mode = Mode.Previous();
			return;
		}
		
		public override int NumberOfModes ()
		{
			return Enum.GetNames(typeof(UltraSonicMode)).Length;
		
		}
        
        public override string SelectedMode ()
		{
			return Mode.ToString();
		}
		
	}
}

