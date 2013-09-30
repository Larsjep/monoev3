using System;

namespace MonoBrickFirmware.Native
{
	public class Ev3Device
	{
		private UnixDevice device;
		private MemoryArea memory;
		
		public Ev3Device(string devicePath, uint memorySize, int memoryOffset)
		{
			device = new UnixDevice(devicePath);
			memory =  device.MMap(memorySize, memoryOffset);	
		
		}
		
		public void Write (int offset, byte[] data)
		{
			memory.Write(offset, data);
		
		}
		
		
		public void Write (DeviceCommand command)
		{
			Write(0,command);	
		}
		
		public void Write (int offset, DeviceCommand command)
		{
			Write(offset, command.Data);
		}
		
		public DeviceReply Read ()
		{
			return Read(0);
		}
		
		public DeviceReply Read (int offset)
		{
			return null;
		
		}
		
		
	}
}

