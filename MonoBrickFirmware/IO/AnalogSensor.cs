using System;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.IO
{
	
	/// <summary>
	/// Analog commands
	/// </summary>
	[Flags]
	public enum AnalogMode  {
		#pragma warning disable 
		None = (byte)'-', Float = (byte)'f', Set = (byte)'0', ColorFull = 0x0D, ColorRed = 0x0E, 
		ColorGreen = 0x0F, ColorBlue = 0x10, ColorNone = 0x11, ColorExit = 0x12, Pin1 = 0x01, Pin5 = 0x02
		#pragma warning restore
	};
	
	public abstract class AnalogSensor: Input{
		private UnixDevice deviceManager;
		
		
		protected const int ADCResolution = 4095;//12-bit
		protected AnalogMode AnalogMode{get;private set;}
		
		//Analog memory offsets
    	private const int PinOneOffset = 0;
    	private const int PinSixOffset = 8;
    	private const int PinFiveOffset = 16;
    	private const int BatteryTempOffset = 24;
    	private const int MotorCurrentOffset = 26;
   	 	private const int BatteryCurrentOffset = 28;
    	private const int BatteryVoltageOffset = 30;
    	
		
		public AnalogSensor (SensorPort port):base(port)
		{
			deviceManager =  new UnixDevice("/dev/lms_dcm");
		}
		
		protected bool SetMode(AnalogMode mode)
	    {
	        this.AnalogMode = mode;
	        byte [] modes = new byte[NumberOfSenosrPorts];
	        for(int i = 0; i < modes.Length; i++)
	            modes[i] = (byte)AnalogMode.None;
	        modes[(int)port] = (byte)mode;
	        deviceManager.Write(modes);
	        return true;
	    }
		
		protected Int16 ReadPin1AsPct ()
		{
			return (Int16)((ReadPin1()*100)/ADCResolution);
		}
		
		protected Int16 ReadPin6AsPct ()
		{
			return (Int16)((ReadPin6()*100)/ADCResolution);
		}
		
		protected Int16 ReadPin1()
		{
		    DeviceReply reply = new DeviceReply(analogMemory.Read(PinOneOffset, NumberOfSenosrPorts*2));
		    return reply.GetInt16((int) port * 2);
		}
		
		protected Int16 ReadPin5()
		{
		    DeviceReply reply = new DeviceReply(analogMemory.Read( PinFiveOffset, NumberOfSenosrPorts*2));
		    return reply.GetInt16((int) port * 2);
		}
		
		protected Int16 ReadPin6()
		{
		    DeviceReply reply = new DeviceReply(analogMemory.Read(PinSixOffset, NumberOfSenosrPorts*2));
		    return reply.GetInt16((int) port * 2);
		}
		
		
	}
}

