using System;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// I2C sensor class for writing and reading. Should not be used for inheritance
	/// </summary>
	public sealed class I2CSensor : I2CAbstraction{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.I2CSensor"/> class with I2C address 0x02
		/// </summary>
		/// <param name="port">Sensor port to use</param>
		/// <param name="mode">I2C mode to use</param>
		public I2CSensor (SensorPort port, I2CMode mode): this(port, 0x02, mode)
		{
			
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.I2CSensor"/> class.
		/// </summary>
		/// <param name="port">Sensor port to use.</param>
		/// <param name="address">I2C address to use</param>
		/// <param name="mode">I2C mode to use</param>
		public I2CSensor (SensorPort port, byte address, I2CMode mode):base(port,address, mode)
		{
			base.Initialise();
		
		}
		
		/// <summary>
		/// Reads a 8 byte register from the sensor
		/// </summary>
		/// <returns>
		/// The bytes that was read
		/// </returns>
		/// <param name='register'>
		/// Register to read
		/// </param>
		public new byte[] ReadRegister(byte register){
			return base.ReadRegister(register);
		}

		/// <summary>
		/// Reads a register from the sensor
		/// </summary>
		/// <returns>
		/// The bytes that was read
		/// </returns>
		/// <param name='register'>
		/// Register to read
		/// </param>
		/// <param name='rxLength'>
		/// The number of bytes to read
		/// </param>
  		public new byte[] ReadRegister(byte register, byte rxLength)
        {
           	return base.ReadRegister(register,rxLength);
        }

		/// <summary>
		/// Writes a byte to a register.
		/// </summary>
		/// <param name='register'>
		/// Register to write to
		/// </param>
		/// <param name='data'>
		/// Data byte to write
		/// </param>
		public new void WriteRegister(byte register, byte data) {
           base.WriteRegister(register,data);
        }
        
        /// <summary>
        /// Writes the register.
        /// </summary>
        /// <param name="register">Register to write to</param>
        /// <param name="data">Data bytes to write</param>
        public new void WriteRegister(byte register, byte[] data) {
           base.WriteRegister(register,data);
        }
	}
	
	
	
}

