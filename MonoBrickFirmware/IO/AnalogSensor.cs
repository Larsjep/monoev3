using System;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.IO
{
	public class AnalogSensor: Input{
	
		public AnalogSensor (SensorPort port):base(port)
		{
			
		
		}
		
		protected Int16 ReadPin1()
		{
		    DeviceReply reply = new DeviceReply(analogMemory.Read(PinOneOffset, NumberOfSenosrPorts*2));
		    return reply.GetInt16((int) port * 2);
		}
		
		protected Int16 ReadPin6()
		{
		    DeviceReply reply = new DeviceReply(analogMemory.Read(PinSixOffset, NumberOfSenosrPorts*2));
		    return reply.GetInt16((int) port * 2);
		}
		
		protected Int16 ReadPin5()
		{
		    DeviceReply reply = new DeviceReply(analogMemory.Read( PinFiveOffset, NumberOfSenosrPorts*2));
		    return reply.GetInt16((int) port * 2);
		}
	}
}

