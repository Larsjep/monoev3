using System;

namespace MonoBrickFirmware.IO
{
	public class PCF8574: I2CAbstraction, ISensor
	{
		public PCF8574 (SensorPort port, byte address) : base (port, address, I2CMode.LowSpeed9V)
		{
			base.Initialise();
		}
		
		/// <summary>
		/// Read the pins from the sensor (0-255)
		/// </summary>
        public int Read()
        {
            return base.ReadRegister(I2CAddress)[0];
        }

		/// <summary>
		/// Write to sensor 
		/// </summary>
		/// <param name='set'>
		/// Pins to set (0-255)
		/// </param>
        public void Write(byte set) {
            base.WriteRegister(I2CAddress,set);
        }

		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
        public string ReadAsString()
        {
            return "I/O value: " + Read();
        }
		
	}
}

