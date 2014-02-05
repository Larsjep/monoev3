using System;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.Sensors
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
		Unknown = 111, DaisyChain = 117, NXTColor = 118, NXTDumb = 119, NXTI2C = 120, InputResistor = 121, 
		UART = 122, OutputResistor = 123, OutputCommunication = 124, Tacho = 125, None = 126, Error = 127 	
		#pragma warning restore
	};
	
	
	/// <summary>
	/// Class used to share sensor functions and informations between sensor instances
	/// </summary>
	public sealed class SensorManager
	{
		private static byte [] sensorData = new byte[3*NumberOfSenosrPorts];
		private static object setupLock = new object();
		
		//Analog memory offsets
		private const int TypeOffset = 5156;
    	private const int ConnectionOffset = 5160;
		private static readonly SensorManager instance = new SensorManager();
		
		private const uint i2cMemorySize = 42748;
		private const uint analogMemorySize = 5172;  
		private const uint uartMemorySize = 42744; 
		
		// I2C IO control
		private const UInt32 I2CIOSetConnection = 0xc00c6902;
    	private const UInt32 I2CIOReadTypeInfo = 0xc03c6903;
    	private const UInt32 I2CIOSet = 0xc02c6906;
		
		// UART IO control
		private const UInt32 UartIOSetConnection = 0xc00c7500;//This number can also be found in sensormanager.cs
		private const UInt32 UartIOReadModeInfo = 0xc03c7501;
    	private const UInt32 UartIONackModeInfo = 0xc03c7502;
    	private const UInt32 UartIOClearChanges = 0xc03c7503;
		
		public const int NumberOfSenosrPorts = 4;
		
		private UnixDevice DeviceManager{get; set;}
		
		private UnixDevice AnalogDevice{get;set;}
		public MemoryArea AnalogMemory{get; private set;}
		
		private UnixDevice UartDevice{get; set;}
		public MemoryArea UartMemory{get; private set;}
		
		public UnixDevice I2CDevice{get; private set;}
		public MemoryArea I2CMemory{get; private set;}
		
		private SensorManager ()
		{
			DeviceManager =  new UnixDevice("/dev/lms_dcm");
			
			AnalogDevice = new UnixDevice("/dev/lms_analog");
			AnalogMemory = AnalogDevice.MMap(analogMemorySize,0);
			
			UartDevice = new UnixDevice("/dev/lms_uart");
			UartMemory = UartDevice.MMap(uartMemorySize,0);
			
			I2CDevice = new UnixDevice("/dev/lms_iic");
			I2CMemory = I2CDevice.MMap(i2cMemorySize,0);
			
		}
		
		public static SensorManager Instance
		{
			get{return instance;}
		}
		
		
		private byte[] SetupCommand(SensorPort sensorPort, ConnectionType conn, SensorType type, byte mode)
		{
        	lock (setupLock) {
				sensorData [(int)sensorPort] = (byte)conn;
				sensorData [(int)sensorPort + NumberOfSenosrPorts] = (byte)type;
				sensorData [(int)sensorPort + 2 * NumberOfSenosrPorts] = mode;
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
				UartDevice.IoCtl ((Int32)UartIOSetConnection, SetupCommand (port, ConnectionType.None, SensorType.None,0));
			}
		}
		
		public void SetUartOperatingMode (UARTMode mode, SensorPort port)
		{
			unchecked {
				UartDevice.IoCtl ((Int32)UartIOSetConnection,  SetupCommand (port, ConnectionType.UART, SensorType.None,(byte) mode));
			}
	    }
	    
	    public void ClearUartPortChanged(SensorPort port)
		{
			unchecked {
				UartDevice.IoCtl ((Int32)UartIOClearChanges,  SetupCommand (port, ConnectionType.UART, SensorType.None, (byte)UARTMode.Mode0));
			}
	    }
	    
    	public void SetI2COperatingMode(SensorPort port)
    	{
        	unchecked {
        		I2CDevice.IoCtl((Int32) I2CIOSetConnection, SetupCommand(port,ConnectionType.NXTI2C, SensorType.I2CUnknown, 255)); 
        	}
        	        
    	}
    	
    	public void ResetI2C(SensorPort port)
   	 	{
        	unchecked {
        		I2CDevice.IoCtl((Int32) I2CIOSetConnection,SetupCommand(port, ConnectionType.None, SensorType.None, 0));
        	}        
    	}

	    
	    public void SetAnalogMode(AnalogMode mode, SensorPort port)
	    {
	        byte [] modes = new byte[SensorManager.NumberOfSenosrPorts];
	        for(int i = 0; i < modes.Length; i++)
	            modes[i] = (byte)AnalogMode.None;
	        modes[(int)port] = (byte)mode;
	        DeviceManager.Write(modes);
	    }
		
		
	}
	
}

