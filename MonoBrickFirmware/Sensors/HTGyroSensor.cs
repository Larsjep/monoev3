using System;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// HiTechnic gyro sensor
    /// </summary>
    public class HiTecGyro : AnalogSensorAbstraction, ISensor {
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.NXT.HiTecGyro"/> class without offset
		/// </summary>
		public HiTecGyro(SensorPort port) : this(port,0) {
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.NXT.HiTecGyro"/> class.
		/// </summary>
		/// <param name='offset'>
		/// Offset
		/// </param>
		public HiTecGyro(SensorPort port, int offset) : base(port) {
			Offset = offset;
			base.SetMode(AnalogMode.Set); 
		}

		/// <summary>
		/// Read angular acceleration
		/// </summary>
		public int ReadAngularAcceleration()
        {
            return base.ReadPin1As10Bit() - Offset;
        }

		/// <summary>
		/// Reads the angular acceleration as a string.
		/// </summary>
		/// <returns>
		/// The value as a string.
		/// </returns>
		public string ReadAsString()
        {
            return this.ReadAngularAcceleration().ToString() + " deg/sec";
        }

		/// <summary>
		/// Gets or sets the offset.
		/// </summary>
		/// <value>
		/// The offset.
		/// </value>
		public int Offset{get;set;}

    }
}

