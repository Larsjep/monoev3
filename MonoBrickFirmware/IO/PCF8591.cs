using System;

namespace MonoBrickFirmware.IO
{
	
	/// <summary>
	/// ADC port for use with the PCF8591 I2C chip
	/// </summary>
    public enum ADCPort {
		#pragma warning disable 
		Port0 =0x00, Port1 = 0x01, Port2 = 0x02, Port3 = 0x03
		#pragma warning restore
	}
	
	/// <summary>
	/// PCF8591 chip with four input and four output ports
	/// </summary>
    public class PCF8591: I2CAbstraction, ISensor
	{
		public PCF8591 (SensorPort port, byte address) : base (port, address, I2CMode.LowSpeed9V)
		{
			base.Initialise();
		}
		
		/// <summary>
		/// Read the value on the specified port
		/// </summary>
		/// <param name='port'>
		/// Port to read from
		/// </param>
        public byte Read(ADCPort port) {
            return ReadRegister((byte)port)[0];
        }

		/// <summary>
		/// Write to the chip
		/// </summary>
		/// <param name='port'>
		/// Port to write to
		/// </param>
		/// <param name='value'>
		/// Value to write
		/// </param>
        public void Write(ADCPort port, byte value) {
            base.WriteRegister((byte) ((byte)(port) |0x40), value);
        }

		/// <summary>
		/// Reads the all ports as a string
		/// </summary>
		/// <returns>
		/// The value of all ports as a string
		/// </returns>
        public string ReadAsString()
        {
            return "P0:" + Read(ADCPort.Port0) + " P1:" + Read(ADCPort.Port1) + " P2:" + Read(ADCPort.Port2) + " P3:" + Read(ADCPort.Port3);
        }
		
		
	}
}

