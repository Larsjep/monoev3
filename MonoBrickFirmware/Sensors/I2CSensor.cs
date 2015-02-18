using System;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Tools;
using System.Text;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Sensor modes
	/// </summary>
	public enum I2CMode {
		#pragma warning disable 
		LowSpeed = AnalogMode.Set, LowSpeed9V = AnalogMode.Set | AnalogMode.Pin1
		#pragma warning restore
	};
	
	/// <summary>
	/// Base class for all I2C sensors. This should be used when implementing a new I2C sensor
	/// </summary>
	public abstract class I2CSensor:ISensor
	{
		private UnixDevice I2CDevice;
		//private MemoryArea I2CMemory;
		
		private const int InitDelay = 100;
		private const int BufferSize = 30;
		
		//I2C control 
		private const UInt32 I2CIOSetup = 0xc04c6905;

		private enum I2cRegister : byte
		{
			FirmwareVersion = 0x00, VendorId = 0x08, DeviceId = 0x10
		};

		private string ConvertByteArrayToString (byte[] bytes)
		{
			string s = System.Text.Encoding.Default.GetString(bytes);
			int pos = s.IndexOf('\0');  
			if (pos >= 0)
    			s = s.Substring(0, pos);
			return s;
		}
		
		protected byte I2CAddress = 0x00;
		
		protected const int NumberOfSensorPorts = SensorManager.NumberOfSensorPorts;
		protected SensorPort port;
		protected UARTMode uartMode{get; private set;}
		protected I2CMode mode;
		
		public I2CSensor (SensorPort port, byte address, I2CMode mode)
		{
			this.port = port;
			this.I2CAddress = address;
			I2CDevice = SensorManager.Instance.I2CDevice;
			this.mode = mode;
			SensorManager.Instance.SetAnalogMode((AnalogMode)mode, port);
		}

		protected string GetDeviceId ()
		{
			return ConvertByteArrayToString(ReadRegister((byte)I2cRegister.DeviceId));
		}

		protected string GetVendorId ()
		{
			return ConvertByteArrayToString(ReadRegister((byte)I2cRegister.VendorId));
		}

		protected string GetFirmwareVersion ()
		{
			return ConvertByteArrayToString(ReadRegister((byte)I2cRegister.FirmwareVersion));
		}

		protected void Reset ()
		{
			SensorManager.Instance.ResetI2C(this.port);
		}
		
		protected void SetMode ()
		{
			SensorManager.Instance.SetI2COperatingMode(this.port);
		}
		
		protected bool Initialise()
	    {
	       	SensorManager.Instance.SetAnalogMode(AnalogMode.Pin5, this.port);
	       	Reset();
	       	System.Threading.Thread.Sleep(InitDelay);
	       	SetMode();
	       	System.Threading.Thread.Sleep(InitDelay);
	       	SetMode();
	       	System.Threading.Thread.Sleep(InitDelay);
	       	return true;
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
		protected byte[] ReadRegister(byte register){
			return ReadRegister(register,8);
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
  		protected byte[] ReadRegister(byte register, byte rxLength)
        {
           	byte[] command = {};
           	return WriteAndRead(register, command, rxLength);
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
		protected void WriteRegister(byte register, byte data) {
            //byte[] command = { I2CAddress, register, data};
            byte[] command = {data};
            WriteAndRead(register, command, 0);
        }
        
        protected void WriteRegister(byte register, byte[] data) {
            WriteAndRead(register, data, 0);
        }
        

       	/// <summary>
		/// Write and read an array of bytes to the sensor
		/// </summary>
		/// <returns>The bytes that was read</returns>
		/// <param name="register">Register to write to.</param>
		/// <param name="data">Byte array to write</param>
		/// <param name="rxLength">Length of the expected reply</param>
        protected byte[] WriteAndRead (byte register, byte[] data, int rxLength)
		{
			if (rxLength > BufferSize)
				throw new ArgumentOutOfRangeException("I2C Receive Buffer only holds " + BufferSize + " bytes");
			if (data.Length > BufferSize) {
				throw new ArgumentOutOfRangeException("I2C Write Buffer only holds " + BufferSize + " bytes");
			}
			bool dataReady = false;
			int replyIndex = 0;
			byte[] writeData = new byte[BufferSize];//30
			Array.Copy (data, 0, writeData, 0, data.Length);
			ByteArrayCreator command = new ByteArrayCreator ();
			command.Append ((int)-1);
			command.Append ((byte)this.port);
			command.Append ((byte)1);//repeat
			command.Append ((short)0);//time
			command.Append ((byte)(data.Length + 2));//length of write data
			command.Append ((byte)((byte)I2CAddress >> 1));
			command.Append (register);
			command.Append(writeData);
			command.Append ((byte)-rxLength);
			replyIndex = command.Data.Length;
			command.Append (new byte[BufferSize]);//make room for reply
			byte[] i2cData = command.Data;
			while (!dataReady) {
				unchecked {
					I2CDevice.IoCtl ((Int32)I2CIOSetup, i2cData);
				}
				int status = BitConverter.ToInt32 (i2cData, 0);
				if (status < 0) {
					throw new Exception ("I2C I/O error");
				}
				if (status == 0) {
					byte[] reply = new byte[rxLength];
					if (rxLength > 0) {
						Array.Copy(i2cData,replyIndex, reply,0, rxLength);
					}
					return reply;
				}
			}
        	throw new TimeoutException("I2C timeout");
    	}
    	
    	public abstract string ReadAsString ();
    	
		public abstract void SelectNextMode();
		
		public abstract string GetSensorName();
		
		public abstract void SelectPreviousMode();
		
		public abstract int NumberOfModes();
        
        public abstract string SelectedMode();
        
        public SensorPort Port{ get {return port;}}
    	
	}
}

