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
		Mode0 = 0, Mode1 = 1, Mode2 = 2, Mode3 = 3, Mode4 = 4, Mode5 = 5, Mode6 = 6, Mode7 = 7	
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
	
	public class Input
	{
		private static byte [] sensorData = new byte[3*NumberOfSenosrPorts];
		private static object setupLock = new object();
		
		protected const int NumberOfSenosrPorts = 4;
		
		protected UnixDevice managerDevice;
		protected UnixDevice analogDevice;
		
		protected MemoryArea analogMemory;
		protected MemoryArea connectionMemory;
		protected MemoryArea typeMemory;
		
		
		protected const int AnalogMemorySize = 5172;
		protected const int ConnectionMemorySize = NumberOfSenosrPorts;
		protected const int TypeMemorySize =  NumberOfSenosrPorts;
    	
    	//Analog constants 
    	protected const int PinOneOffset = 0;
    	protected const int PinSixOffset = 8;
    	protected const int PinFiveOffset = 16;
    	protected const int BatteryTempOffset = 24;
    	protected const int MotorCurrentOffset = 26;
   	 	protected const int BatteryCurrentOffset = 28;
    	protected const int BatteryVoltageOffset = 30;
    	protected const int TypeOffset = 5156;
    	protected const int ConnectionOffset = 5160;
		
		protected SensorPort port;
		
		public Input (SensorPort port)
		{
			managerDevice =  new UnixDevice("/dev/lms_dcm");
			analogDevice = new UnixDevice("/dev/lms_analog");
			analogMemory = analogDevice.MMap(AnalogMemorySize,0);
			//connectionMemory = analogDevice.MMap(ConnectionMemorySize,ConnectionOffset);
			//typeMemory = analogDevice.MMap(TypeOffset,TypeMemorySize);
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
		
		public void SetSensorMode(SensorMode mode)
	    {
	        byte [] modes = new byte[NumberOfSenosrPorts];
	        for(int i = 0; i < modes.Length; i++)
	            modes[i] = (byte)'-';
	        modes[(int)port] = (byte)mode;
	        managerDevice.Write(modes);
	    }
	    
	    public SensorType GetAnalogSensorType ()
		{
			return (SensorType) analogMemory.Read()[(int) port + TypeOffset];
		}
		
		public ConnectionType GetConnectionType (){
			return (ConnectionType) analogMemory.Read()[(int) port+ ConnectionOffset]; 
		}
	}
	
	
	
	public class AnalogSensor: Input{
	
		public AnalogSensor (SensorPort port):base(port)
		{
			
		
		}
		
		public Int16 ReadPin1()
		{
		    DeviceReply reply = new DeviceReply(analogMemory.Read());
		    return reply.GetInt16((int) port * 2 + PinOneOffset);
		}
		
		public Int16 ReadPin6()
		{
		    DeviceReply reply = new DeviceReply(analogMemory.Read());
		    return reply.GetInt16((int) port * 2 + PinSixOffset);
		}
		
		public Int16 ReadPin5()
		{
		    DeviceReply reply = new DeviceReply(analogMemory.Read());
		    return reply.GetInt16((int) port * 2 + PinFiveOffset);
		}
	}
	
	
	
	
	
	
}

