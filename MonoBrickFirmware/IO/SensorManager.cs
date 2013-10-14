using System;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.IO
{
	
	/// <summary>
    /// Sensor ports
    /// </summary>
    public enum SensorPort  {
		#pragma warning disable 
		In1 = 0, In2 = 1, In3 = 2, In4 = 3 
		#pragma warning restore
	};
	
	/// <summary>
	/// Device types
	/// </summary>
	public enum SensorType  {
		#pragma warning disable 
		NXTTouch = 1, NXTLight = 2, NXTSound = 3, NXTColor = 4, NXTUltraSonic = 5, NXTTemperature = 6, LMotor = 7 , MMotor = 8,
		Touch = 16, Test = 21, Color = 29, UltraSonic = 30, Gyro = 32, IR = 33, I2CUnknown = 100, NXTTest = 101, NXTI2c = 123, 
		Terminal = 124, Unknown = 125,  None = 126, Error = 127 
		#pragma warning restore
	};
	
	
	/// <summary>
	/// Sensor modes
	/// </summary>
	public enum SensorMode {
		#pragma warning disable 
		Mode0 = 0, Mode1 = 1, Mode2 = 2, Mode3 = 3, Mode4 = 4, Mode5 = 5, Mode6 = 6	
		#pragma warning restore
	};
	
	/// <summary>
	/// Sensor modes
	/// </summary>
	public enum ConnectionType {
		#pragma warning disable 
		Unknown = 111, DaisyChain = 117, NXTColor = 118, NXTDumb = 119, NXTI2c = 120, InputResistor = 121, 
		UART = 122, OutputResistor = 123, OutputCommunication = 124, Tacho = 125, None = 126, Error = 127 	
		#pragma warning restore
	};
	
	internal class SensorManager
	{
		private const int NumberOfSenosrPorts = 4;
		protected UnixDevice managerDevice;
		protected UnixDevice analogDevice;
		protected MemoryArea analogMemory;
		
		protected const int AnalogMemorySize = 5172;
    	//Analog constants 
    	protected const int Pin1Offset = 0;
    	protected const int Pin6Offset = 8;
    	protected const int Pin5Offset = 16;
    	protected const int BatteryTempOffset = 24;
    	protected const int MotorCurrentOffset = 26;
   	 	protected const int BatteryCurrentOffset = 28;
    	protected const int BatteryVoltageOffset = 30;
    	protected const int IndcmOffset = 5156;
    	protected const int ConnectionOffset = 5160;
		
		public SensorManager ()
		{
			managerDevice =  new UnixDevice("/dev/lms_dcm");
			analogDevice = new UnixDevice("/dev/lms_analog");
			analogMemory = analogDevice.MMap(AnalogMemorySize,0); 
		}
		
		public void SetSensorMode(SensorPort port, SensorMode mode)
	    {
	        byte [] modes = new byte[NumberOfSenosrPorts];
	        for(int i = 0; i < modes.Length; i++)
	            modes[i] = (byte)'-';
	        modes[(int)port] = (byte)mode;
	        managerDevice.Write(modes);
	    }
	    
	    public SensorType GetAnalogSensorType (SensorPort port)
		{
			return (SensorType) analogMemory.Read(IndcmOffset + (int) port, 1)[0];
		}
		
		public ConnectionType GetConnectionType (SensorPort port){
			return (ConnectionType) analogMemory.Read(ConnectionOffset + (int) port, 1)[0];
		}
		
	}
}

