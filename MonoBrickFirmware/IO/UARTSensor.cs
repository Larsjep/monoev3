using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.IO
{
		/// <summary>
	/// Class for reading and writing data to a UART port
	/// </summary>
	public class Uart: Input
	{
		protected UnixDevice uartDevice;
		protected MemoryArea uartMemory;
		
		private const int UartMemorySize = 42744;
		private const int WaitTimout = 5000;
		private const int WaitFrequency = 1000;
		private const int InitRetry = 100;
		private const int InitDelay = 50;
		private const int OpenRetry = 5;
		
		protected const int UartStatusOffset = 42608;
    	protected const int UartActualOffset = 42592;
    	protected const int UartRawOffset = 4192;
    	
    	protected const int UartRawDataSize = 32; //DevRawSize2
		protected const int UartRawBufferLength = 300;
		protected const int UartRawBufferSize = UartRawDataSize * UartRawBufferLength;//DevRawSize1
    	
		// IO control
		protected const UInt32 UartIOSetConnection = 0xc00c7500;
		protected const UInt32 UartIOReadModeInfo = 0xc03c7501;
    	protected const UInt32 UartIONackModeInfo = 0xc03c7502;
    	protected const UInt32 UartIOClearChanges = 0xc03c7503;
    
    	protected const byte UartPortChanged = 1;
    	protected const byte UartDataReady = 8;
		
		public Uart (SensorPort port):base(port)
		{
			uartDevice = new UnixDevice("/dev/lms_uart");
			uartMemory = uartDevice.MMap(UartMemorySize,0);
		}
		
		private byte[] GetRawData ()
		{
			var data = new byte[NumberOfSenosrPorts * UartRawBufferSize]; 
			Array.Copy(uartMemory.Read(), UartRawOffset, data, 0,NumberOfSenosrPorts * UartRawBufferSize);
			return data;  
		}
		
		private byte[] GetActualData ()
		{
			var data = new byte[NumberOfSenosrPorts * 2]; 
			Array.Copy(uartMemory.Read(), UartActualOffset, data, 0,NumberOfSenosrPorts * 2);
			return data; 	
		
		}
		
		private byte[] GetStatusData ()
		{
			var data = new byte[NumberOfSenosrPorts]; 
			Array.Copy(uartMemory.Read(), UartStatusOffset, data, 0,NumberOfSenosrPorts);
			return data; 	
		}
		
		
		private void CheckSensor()
    	{
	        if(GetConnectionType() != ConnectionType.UART)
	        	throw new Exception("UART sensor is not connected");
    	}
	    
		public void Reset ()
		{
			// Force the device to reset
			unchecked {
				uartDevice.IoCtl ((Int32)UartIOSetConnection, SetupCommand (this.port, ConnectionType.None, SensorType.None, SensorMode.Mode0));
			} 
    	}
	    
		public int CalcRawOffset()
    	{
        	return  (int)port * UartRawBufferSize * GetActualData()[(int) port * 2] * UartRawDataSize;
    	}
    	
    	public byte GetStatus()
	    {
	        return GetStatusData()[(int)port];
	    }
		
		
	    public byte WaitNonZeroStatus (int timeout)
		{
			int target = timeout/WaitFrequency;
			int time = 0;
	        byte status = 0;
			while (time < target) {
				System.Threading.Thread.Sleep(WaitFrequency);
				status = GetStatus();
				if(status != 0 || GetConnectionType() != ConnectionType.UART)
					break;
				time = time + WaitFrequency;
			}
			return status;
	    }
	    
	    
	    public byte WaitZeroStatus(int timeout)
	    {
	        int target = timeout/WaitFrequency;
	        int time = 0;
	        byte status = GetStatus();
	        while (time < target)
	        {
	            if (status == 0)
	                return status;
	            if (GetConnectionType() !=  ConnectionType.UART)
	                return status;
	            System.Threading.Thread.Sleep(WaitFrequency);
	            timeout = timeout + WaitFrequency;
	            status = GetStatus();
	        }
	        return status;       
	    }

	    
		public void SetOperatingMode (SensorMode mode)
		{
			unchecked {
				int checkValue = uartDevice.IoCtl ((Int32)UartIOSetConnection, SetupCommand (port, ConnectionType.UART, SensorType.None, mode));
				Console.WriteLine("Check value: " + checkValue);
			} 
	    }
		
		
	    /*public bool GetModeInfo (SensorMode mode, RawDeviceInformation deviceInformation)
		{
	        
			deviceInformation.Port = (byte)port;
			deviceInformation.Mode = (byte)mode;
	        
			//System.out.println("size is " + uc.size() + " TYPES " + uc.TypeData.size() + " ptr " + uc.getPointer().SIZE);
			byte[] data = deviceInformation.ToByteArray ();
			unchecked {
				uartDevice.IoCtl ((Int32)UartIOReadModeInfo, data);
			}
	        //uart.ioctl(UART_READ_MODE_INFO, uc.getPointer());
	        //uc.read();
	        return data[0] != 0;//Data has been placed in the structure
	    }*/
	    
	    
	    
	    /*public void ClearModeCache(SensorMode mode, RawDeviceInformation deviceInformation)
	    {
	        deviceInformation.Port = (byte)port;
	        deviceInformation.Mode = (byte)mode;
	        //System.out.println("size is " + uc.size() + " TYPES " + uc.TypeData.size() + " ptr " + uc.getPointer().SIZE);
	        //uart.ioctl(UART_NACK_MODE_INFO, deviceInformation.getPointer());
	        unchecked {
				uartDevice.IoCtl ((Int32)UartIONackModeInfo, deviceInformation.ToByteArray());
			}
	    }*/
		
		
	    public void ClearPortChanged()
		{
			unchecked {
				uartDevice.IoCtl ((Int32)UartIOClearChanges, SetupCommand (port, ConnectionType.UART, SensorType.None, SensorMode.Mode0));
				uartMemory.Write (UartStatusOffset, new byte[] { (byte)(uartMemory.Read ((int)port, 1) [0] & ~UartPortChanged) });
			}
	    }
		
	    public bool InitSensor(SensorMode mode)
	    {
	        byte status;
	        int retryAttemps = 0;
	        //long base = System.currentTimeMillis();
	        if (GetConnectionType() != ConnectionType.UART)
	            return false;
	        // now try and configure as a UART
	        
	        SetOperatingMode(mode);
	        status = WaitNonZeroStatus(WaitTimout);
	        while((status & UartPortChanged) != 0 && retryAttemps++ < InitRetry)
	        {
	            // something change wait for it to become ready
	            if (GetConnectionType() != ConnectionType.UART)
	            	return false;
	            ClearPortChanged();
	            System.Threading.Thread.Sleep(InitDelay);
	            status = WaitNonZeroStatus(WaitTimout);
	            if ((status & UartDataReady) != 0 && (status & UartPortChanged) == 0) 
	            {
	                // device ready make sure it is now in the correct mode
	                SetOperatingMode(mode);
	                status = WaitNonZeroStatus(WaitTimout);
	            }
	        }
	        if ((status & UartDataReady) != 0 && (status & UartPortChanged) == 0)
	            return true;
	        else
	            return false;
	    }
		
	    public bool InitialiseSensor(SensorMode mode)
	    {
	        for(int i = 0; i < OpenRetry; i++)
	        {
	            if (GetConnectionType() != ConnectionType.UART)
	                return false;
	            // initialise the sensor, if we have no mode data
	            // then read it, otherwise use what we have
	            if (InitSensor(mode))
	            {
	                return true;
	            }
	            ResetSensor();
	        }
	        return false;
	    }
	    
	    
	    public void ResetSensor()
	    {
	        Reset();
	        WaitZeroStatus(WaitTimout);
	    }
		
		public bool Open()
	    {
	        // clear mode data cache
	        if (InitialiseSensor(SensorMode.Mode0))
	            return true;
	        return false;
	    }
	
	    public void Close()
	    {
	        Reset();
	    }
	    
	    public bool SetMode(SensorMode mode)
	    {
	        
	        SetOperatingMode(mode);
	        int status = WaitNonZeroStatus(WaitTimout);
	        if ((status & UartDataReady) != 0 && (status & UartPortChanged) == 0)
	        {
	            return true;
	        }
	        else
	            // Sensor may have reset try and initialise it in the new mode.
	            return InitialiseSensor(mode);
	    }
	
	   	
	    public byte ReadByte()
	    {
	        CheckSensor();
	        return GetRawData()[CalcRawOffset()];  
	    }
	    
	    public byte[] ReadBytes(int len)
	    {
	        return ReadBytes(0, len);
	    }
	
	    public byte[] ReadBytes (int offset, int len)
		{
			CheckSensor ();
			byte[] data = new byte[len];
			byte[] rawData = GetRawData ();
			Console.WriteLine("Analyzing  " + rawData.Length + " bytes");
			for(int i = 0; i < rawData.Length; i++) {
				if (rawData[i] != 0) {
					Console.WriteLine("Index " + i + " has value " + rawData[i]);
				}
			}
	        Array.Copy(rawData, CalcRawOffset() + offset, data,0,len);
	        return data;
	    }
	    
	    public void ResetAll ()
		{
			// reset everything
			byte[] data = null;
			for (int i = 0; i < NumberOfSenosrPorts; i++) 
			{
				data = SetupCommand ((SensorPort)i, ConnectionType.None, SensorType.None, SensorMode.Mode0);
			}
	        unchecked {
				uartDevice.IoCtl ((Int32)UartIOSetConnection,data);
			} 
	    }
	    
	    
	    
	    /*protected bool ReadModeInfo()
	    {
	        long longBase = System.DateTime.Now.Millisecond;
	        int modeCount = 0;
	        int size = Enum.GetNames(typeof(SensorMode)).Length;
	        while(modeCount < size)
	        {
	            
	            RawDeviceInformation deviceInfo = new RawDeviceInformation();
	            if(GetModeInfo((SensorMode) modeCount, deviceInfo))
	            {
	                ClearModeCache((SensorMode)modeCount, deviceInfo);
	                modeInfo[i] = uc.TypeData;
	                modeCount++;
	            }
	            else{
	                modeInfo[i] = null;
	            }
	            modeCount++;
	        }
	        return modeCount > 0;
	
	    }
	    
	    
	    public String GetModeName(int mode)
	    {
	        if (modeInfo[mode] != null)
	            return new String(modeInfo[mode].Name);
	        else 
	            return "Unknown";
	    }*/
	    
	    /*
	    private byte[] GetDeviceStatus ()
		{
			return UartMemory.Read(UartStatusOffset, NumberOfSenosrPorts);
		
		}
		
		private byte[] GetActual ()
		{
			return UartMemory.Read(UartActualOffset, NumberOfSenosrPorts * 2);
		}
		
		private byte[] GetRaw ()
		{
			return UartMemory.Read(UartRawOffset, NumberOfSenosrPorts * UartRawBufferSize);
		}*/
		
	}
	
	
	
    /// <summary>
    /// Raw device information.
    /// </summary>
    /*public class RawDeviceInformation
    {
        public byte[] Name = new byte[12];
        public byte Type;
        public byte Connection;
        public byte Mode;
        public byte DataSets;
        public byte Format;
        public byte Figures;
        public byte Decimals;
        public byte Views;
        public float RawMin;
        public float RawMax;
        public float PctMin;
        public float PctMax;
        public float SiMin;
        public float SiMax;
        public short InvalidTime;
        public short IdValue;
        public byte  Pins;
        public byte[] Symbol = new byte[5];
        public short Align;
        public byte Port;
        public byte SensorMode;
        public byte[] ToByteArray(){
        	DeviceCommand command = new DeviceCommand();
        	command.Append(Name);
        	command.Append(Type);
        	command.Append(Connection);
        	command.Append(Mode);
        	command.Append(DataSets);
        	command.Append(Format);
        	command.Append(Figures);
        	command.Append(Decimals);
        	command.Append(Views);
        	command.Append(RawMin);
        	command.Append(RawMax);
        	command.Append(PctMin);
        	command.Append(PctMax);
        	command.Append(SiMin);
        	command.Append(InvalidTime);
        	command.Append(IdValue);
        	command.Append(Pins);
        	command.Append(Symbol);
        	command.Append(Port);
        	command.Append(SensorMode);
        	return command.Data;
		}
    }*/
}

