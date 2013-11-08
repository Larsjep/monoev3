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
	
	public abstract class AnalogSensorAbstraction{
		private MemoryArea analogMemory;
		
		
		protected const int ADCResolution = 4095;//12-bit
		protected AnalogMode AnalogMode{get;private set;}
		protected const int NumberOfSenosrPorts = SensorManager.NumberOfSenosrPorts;
		protected SensorPort port;
		
		
		//Analog memory offsets
    	private const int PinOneOffset = 0;
    	private const int PinSixOffset = 8;
    	private const int PinFiveOffset = 16;
    	private const int BatteryTempOffset = 24;
    	private const int MotorCurrentOffset = 26;
   	 	private const int BatteryCurrentOffset = 28;
    	private const int BatteryVoltageOffset = 30;
    	
		
		public AnalogSensorAbstraction (SensorPort port)
		{
			this.port = port;
			SensorManager.Instance.ResetI2C(this.port);
			SensorManager.Instance.ResetUart(this.port);
			analogMemory = SensorManager.Instance.AnalogMemory;
		}
		
		protected void SetMode(AnalogMode mode)
	    {
	        this.AnalogMode = mode;
	        SensorManager.Instance.SetAnalogMode(mode,port);
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
		    return ReadInt16 (PinOneOffset);
		}
		
		protected Int16 ReadPin5()
		{
		    return ReadInt16 (PinFiveOffset);
		}
		
		protected Int16 ReadPin6()
		{
		    return ReadInt16 (PinSixOffset); 
		}
		
		protected byte[] ReadBytes (int offset, int length)
		{
			return analogMemory.Read(offset, length);
		}
		
		private Int16 ReadInt16 (int offset)
		{
			 return BitConverter.ToInt16(analogMemory.Read(offset, NumberOfSenosrPorts*2),(int) port * 2);
		}
		
		
	}
}

