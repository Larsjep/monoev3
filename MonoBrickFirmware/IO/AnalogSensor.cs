using System;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.IO
{
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

