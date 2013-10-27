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
	
	internal sealed class SensorManager
	{
		private static byte [] sensorData = new byte[3*NumberOfSenosrPorts];
		private static object setupLock = new object();
		
		//Analog memory offsets
		private const int TypeOffset = 5156;
    	private const int ConnectionOffset = 5160;
		private static readonly SensorManager instance = new SensorManager();
		
		
		private const uint analogMemorySize = 5172;  
		private const uint UartMemorySize = 42744; 
		private const UInt32 UartIOSetConnection = 0xc00c7500;//This number can also be found in UARTSensor.cs
		
		
		public UnixDevice DeviceManager{get; private set;}
		
		public const int NumberOfSenosrPorts = 4;
		public MemoryArea AnalogMemory{get; private set;}
		public UnixDevice AnalogDevice{get;private set;}
		
		public UnixDevice UartDevice{get; private set;}
		public MemoryArea UartMemory{get; private set;}
		private SensorManager ()
		{
			DeviceManager =  new UnixDevice("/dev/lms_dcm");
			
			AnalogDevice = new UnixDevice("/dev/lms_analog");
			AnalogMemory = AnalogDevice.MMap(analogMemorySize,0);
			
			UartDevice = new UnixDevice("/dev/lms_uart");
			UartMemory = UartDevice.MMap(UartMemorySize,0);
		}
		
		public static SensorManager Instance
		{
			get{return instance;}
		}
		
		
		public 	byte[] SetupCommand(SensorPort sensorPort, ConnectionType conn, SensorType type, UARTMode mode)
		{
        	lock (setupLock) {
				sensorData [(int)sensorPort] = (byte)conn;
				sensorData [(int)sensorPort + NumberOfSenosrPorts] = (byte)type;
				sensorData [(int)sensorPort + 2 * NumberOfSenosrPorts] = (byte)mode;
				return sensorData;
			}
    	}
		
		public SensorType GetSensorType (SensorPort port)
		{
			return (SensorType) AnalogMemory.Read(TypeOffset, NumberOfSenosrPorts)[(int) port];
		}
		
		public ConnectionType GetConnectionType (SensorPort port){
			return (ConnectionType) AnalogMemory.Read(ConnectionOffset,NumberOfSenosrPorts )[(int) port]; 
		}
		
		public void ResetUart (SensorPort port)
		{
			unchecked {
				UartDevice.IoCtl ((Int32)UartIOSetConnection, SensorManager.Instance.SetupCommand (port, ConnectionType.None, SensorType.None, UARTMode.Mode0));
			}
		}
		
	}
	
}

