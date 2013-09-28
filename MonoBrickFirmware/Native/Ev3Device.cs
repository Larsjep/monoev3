using System;

namespace MonoBrickFirmware.Native
{
	public class Ev3Device
	{
		protected UnixDevice Device{get; private set;}
		protected MemoryArea Memory{get; private set;}
		
		public Ev3Device(string devicePath, uint memorySize, int memoryOffset)
		{
			Device = new UnixDevice(devicePath);
			Memory =  Device.MMap(memorySize, memoryOffset);	
		
		}
		
	}
}

