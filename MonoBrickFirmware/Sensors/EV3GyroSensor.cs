using System;
using MonoBrickFirmware.Extensions;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Sensor modes when using a EV3 Gyro sensor
	/// </summary>
	public enum GyroMode { 
		#pragma warning disable 
			/// <summary>
			/// Result will be in degrees
			/// </summary>
			Angle = UARTMode.Mode0,
			/// <summary>
			/// Result will be in degrees per second
			/// </summary>
			AngularVelocity = UARTMode.Mode1,
		#pragma warning restore
	};
	
		
	
	/// <summary>
	/// Class for the EV3 Gyro sensor
	/// </summary>
	public class EV3GyroSensor :UartSensor{

        /// <summary>
        /// Initializes a new instance of the Gyro sensor.
        /// </summary>
        public EV3GyroSensor(SensorPort port, int pollInterval = 50) : this(port, GyroMode.Angle, pollInterval)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Gyro sensor.
        /// </summary>
        /// <param name="mode">Mode.</param>
        public EV3GyroSensor(SensorPort port, GyroMode mode, int pollInterval = 50) : base(port, pollInterval)
        {
            base.Initialise(base.uartMode);
            Mode = mode;
        }

        /// <summary>
        /// Gets or sets the Gyro mode. 
        /// </summary>
        /// <value>The mode.</value>
        public GyroMode Mode {
			get{return (GyroMode) base.uartMode;}
			set{SetMode((UARTMode) value);}
		}
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public override string ReadAsString ()
		{
			string s = "";
			switch ((GyroMode)base.uartMode)
			{
			    case GyroMode.Angle:
			         s = Read().ToString() + " degree";
			        break;
			   case GyroMode.AngularVelocity:
			        s = Read().ToString() +  " deg/sec";
			        break;
			}
			return s;
		}
		
		/// <summary>
		/// Reset the sensor
		/// </summary>
		public new void Reset ()
		{
			if (Mode == GyroMode.Angle) {
				Mode = GyroMode.AngularVelocity;
				System.Threading.Thread.Sleep(100);
				Mode = GyroMode.Angle;
			} 
			else 
			{
				Mode = GyroMode.Angle;
				System.Threading.Thread.Sleep(100);
				Mode = GyroMode.AngularVelocity;
			}
		}
		
		/// <summary>
		/// Get the number of rotations (a rotation is 360 degrees) - only makes sense when in angle mode
		/// </summary>
		/// <returns>The number of rotations</returns>
		public int RotationCount ()
		{
			if (Mode == GyroMode.Angle) {
				return BitConverter.ToInt16(ReadBytes(2),0)/360;
			}
			return 0;	
		}
		
		
		/// <summary>
		/// Read the gyro sensor value. The returned value depends on the mode. 
		/// </summary>
		public override int Read ()
		{
			if (Mode == GyroMode.Angle) {
				return BitConverter.ToInt16(ReadBytes(2),0)%360;
			}
			return BitConverter.ToInt16(ReadBytes(2),0);
		}
		
		public override string GetSensorName ()
		{
			return "EV3 Gyro";
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
			return Enum.GetNames(typeof(GyroMode)).Length;
		
		}
        
        public override string SelectedMode ()
		{
			return Mode.ToString();
		}
		
		
	}
}

