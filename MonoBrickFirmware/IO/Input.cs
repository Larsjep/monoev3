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
	/// Connection modes
	/// </summary>
	public enum ConnectionType {
		#pragma warning disable 
		Unknown = 111, DaisyChain = 117, NXTColor = 118, NXTDumb = 119, NXTI2c = 120, InputResistor = 121, 
		UART = 122, OutputResistor = 123, OutputCommunication = 124, Tacho = 125, None = 126, Error = 127 	
		#pragma warning restore
	};
	
	public abstract class Input
	{
		private static byte [] sensorData = new byte[3*NumberOfSenosrPorts];
		private static object setupLock = new object();
		
		//Analog memory offsets
		private const int TypeOffset = 5156;
    	private const int ConnectionOffset = 5160;
		
		protected const int AnalogMemorySize = 5172;
		protected const int NumberOfSenosrPorts = 4;
		protected UnixDevice analogDevice;
		protected MemoryArea analogMemory;
		protected SensorPort port;
		
		public Input (SensorPort port)
		{
			analogDevice = new UnixDevice("/dev/lms_analog");
			analogMemory = analogDevice.MMap(AnalogMemorySize,0);
			this.port = port;
			 
		}
		
		protected static byte[] SetupCommand(SensorPort sensorPort, ConnectionType conn, SensorType type, SensorMode mode)
		{
        	lock (setupLock) {
				sensorData [(int)sensorPort] = (byte)conn;
				sensorData [(int)sensorPort + NumberOfSenosrPorts] = (byte)type;
				sensorData [(int)sensorPort + 2 * NumberOfSenosrPorts] = (byte)mode;
				return sensorData;
			}
    	}
		
		protected SensorType GetSensorType ()
		{
			return (SensorType) analogMemory.Read(TypeOffset, NumberOfSenosrPorts)[(int) port];
		}
		
		protected ConnectionType GetConnectionType (){
			return (ConnectionType) analogMemory.Read(ConnectionOffset,NumberOfSenosrPorts )[(int) port]; 
		}
	}
	
}

