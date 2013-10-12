using System.Runtime.InteropServices;
using System;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.IO
{	
	public class Uart
	{
		protected UnixDevice uartDevice;
		protected MemoryArea memory;
		private const int DeviceSize = 42744;
		protected const int DeviceStatusOffset = 42608;
    	protected const int DeviceActualOffset = 42592;
    	protected const int DeviceRawOffset = 4192;
    	protected const int DeviceRawSize1Offset = 9600;
    	protected const int DeviceRawSize2Offset = 32;
		
		public Uart ()
		{
			uartDevice = new UnixDevice("/dev/lms_uart");
			memory = uartDevice.MMap(DeviceSize,0);
		}
		
		public void ReadMemory ()
		{
			byte[] data = memory.Read (0, DeviceSize);
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