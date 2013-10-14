using System.Runtime.InteropServices;
using System;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.IO
{	
	
	/// <summary>
	/// Class for reading and writing data to a UART port
	/// </summary>
	public class Uart
	{
		protected UnixDevice uartDevice;
		protected MemoryArea UartMemory;
		
		private const int UartMemorySize = 42744;
		
		protected const int UartStatusOffset = 42608;
    	protected const int UartActualOffset = 42592;
    	protected const int UartRawOffset = 4192;
    	
    	protected const int UartRawDataSize = 32; //DevRawSize2
		protected const int UartRawBufferLength = 300;
		protected const int UartRawBufferSize = UartRawDataSize * UartRawBufferLength;//DevRawSize1
    	
		protected const int NumberOfPorts = 4;
		
		protected SensorManager sensorManager = new SensorManager();
		
		public MotorPort Port{get; private set;}
		
		public Uart (SensorPort port)
		{
			Port = port;
			uartDevice = new UnixDevice("/dev/lms_uart");
			UartMemory = uartDevice.MMap(UartMemorySize,0);
		}
		
		private void CheckSensor()
    	{
	        if(sensorManager.GetConnectionType != ConnectionType.UART)
	        	throw new Exception("UART sensor is not connected");
    	}
	    
		
		protected byte ReadByte()
	    {
	        CheckSensor();
	        return UartMemory.Read(UartRawOffset, CalcRawOffset())[0];
	    }
	    
	    
	    private byte[] GetDeviceStatus ()
		{
			return UartMemory.Read(UartStatusOffset, NumberOfPorts);
		
		}
		
		private byte[] GetActual ()
		{
			return UartMemory.Read(UartActualOffset, NumberOfPorts * 2);
		}
		
		private byte[] GetRaw ()
		{
			return UartMemory.Read(UartRawOffset, NumberOfPorts * UartRawBufferSize);
		}
		
		private int CalcRawOffset()
    	{
        	return  (int)Port * UartRawBufferSize * UartMemory.Read(UartActualOffset, (int) Port * 2) * UartRawDataSize;
    	}
    	
    	private void ReadMemory ()
		{
			byte[] data = UartMemory.Read (0, UartMemorySize);
			var reply = new DeviceReply (data);
			reply.Print();
		}
	    
	    
	    
	    
		
	}
	
	
	
	
	class Sensor
	{
		public Sensor ()
		{
			
		}
	
	}

}