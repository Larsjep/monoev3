using System;

namespace MonoBrickFirmware.Sensors
{
	internal enum CompassRegister : byte
    {
        Version = 0x00, ProductId = 0x08, SensorType = 0x10, Command = 0x41, Degree = 0x42, DegreeHalf = 0x43
    };

	/// <summary>
	/// HiTechnic tilt compass sensor
	/// </summary>
    public class HiTecCompassSensor : I2CSensor
    {
        private const byte CompassAddress = 0x02;
        
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.NXT.HiTecCompass"/> class.
		/// </summary>
		public HiTecCompassSensor(SensorPort port) : base(port, CompassAddress, I2CMode.LowSpeed9V) { 
			base.Initialise();
		}

		/// <summary>
		/// Read the direction of the compass
		/// </summary>
        public int ReadDirection()
        {
            byte[] result = ReadRegister((byte)CompassRegister.Degree, 2);
            return (int) (((int)result[0])*2) + (int) result[1];
        }

		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
        public override string ReadAsString()
        {
            return "Degrees: " + ReadDirection();
        }
        
        public override void SelectNextMode()
		{
			return;
		}
		
		public override string GetSensorName ()
		{
			return "HT Compass";
		}
		
		public override void SelectPreviousMode ()
		{
			
			return;
		}
		
		public override int NumberOfModes ()
		{
			return 1;
		
		}
        
        public override string SelectedMode ()
		{
			return "Angle";
		}

    }

}

