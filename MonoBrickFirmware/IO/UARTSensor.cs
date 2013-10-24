using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.IO
{
	/// <summary>
	/// Class for reading and writing data to a UART port
	/// </summary>
	public class UartSensor: Input
	{
		protected UnixDevice uartDevice;
		protected MemoryArea uartMemory;
		
		private const int UartMemorySize = 42744;
		private const int WaitTimout = 5000;
		private const int WaitFrequency = 500;
		private const int InitRetry = 100;
		private const int InitDelay = 100;
		private const int OpenRetry = 5;
		
		protected const int UartStatusOffset = 42608;
    	protected const int UartActualOffset = 42592;
    	protected const int UartRawOffset = 4192;
    	
    	protected const int UartRawDataSize = 32; 
		protected const int UartRawBufferLength = 300;
		protected const int UartRawBufferSize = UartRawDataSize * UartRawBufferLength;
    	
		// IO control
		protected const UInt32 UartIOSetConnection = 0xc00c7500;
		protected const UInt32 UartIOReadModeInfo = 0xc03c7501;
    	protected const UInt32 UartIONackModeInfo = 0xc03c7502;
    	protected const UInt32 UartIOClearChanges = 0xc03c7503;
    
    	protected const byte UartPortChanged = 1;
    	protected const byte UartDataReady = 8;
		
		protected SensorMode mode = SensorMode.Mode0;
		
		public UartSensor (SensorPort port):base(port)
		{
			uartDevice = new UnixDevice("/dev/lms_uart");
			uartMemory = uartDevice.MMap(UartMemorySize,0);
		}
		
		protected void Reset()
	    {
	        unchecked {
				uartDevice.IoCtl ((Int32)UartIOSetConnection, SetupCommand (this.port, ConnectionType.None, SensorType.None, SensorMode.Mode0));
			}
			mode = SensorMode.Mode0; 
	        WaitZeroStatus(WaitTimout);
	    }
		
		
	    protected bool Initialise(SensorMode mode)
	    {
	        for(int i = 0; i < OpenRetry; i++)
	        {
	            if (GetConnectionType() != ConnectionType.UART)
	                return false;
	            if (InitUart(mode))
	            {
	                return true;
	            }
	            Reset();
	        }
	        return false;
	    }
	    
		protected bool SetMode(SensorMode mode)
	    {
	        
	        SetOperatingMode(mode);
	        int status = WaitNonZeroStatus(WaitTimout);
	        if ((status & UartDataReady) != 0 && (status & UartPortChanged) == 0)
	        {
	            return true;
	        }
	        else
	            return Initialise(mode);
	    }
	
	   	
	    protected byte ReadByte()
	    {
	        CheckSensor();
	        return GetRawData()[CalcRawOffset()];  
	    }
	    
	    protected byte[] ReadBytes(int len)
	    {
	        return ReadBytes(0, len);
	    }
	
	    protected byte[] ReadBytes (int offset, int len)
		{
			CheckSensor ();
			byte[] data = new byte[len];
			byte[] rawData = GetRawData ();
			Array.Copy(rawData, CalcRawOffset() + offset, data,0,len);
	        return data;
	    }
		
		
		private byte[] GetRawData ()
		{
			return uartMemory.Read(UartRawOffset, NumberOfSenosrPorts * UartRawBufferSize);
		}
		
		private byte[] GetActualData ()
		{
			return uartMemory.Read(UartActualOffset, NumberOfSenosrPorts * 2); 
		}
		
		private byte[] GetStatusData ()
		{
			return uartMemory.Read(UartStatusOffset, NumberOfSenosrPorts);
		}
		
		
		private void CheckSensor()
    	{
	        if(GetConnectionType() != ConnectionType.UART)
	        	throw new Exception("UART sensor is not connected");
    	}
	    
		private int CalcRawOffset()
    	{
        	return  (int)port * UartRawBufferSize * GetActualData()[(int) port * 2] * UartRawDataSize;
    	}
    	
    	private byte GetStatus()
	    {
	        return GetStatusData()[(int)port];
	    }
		
		
	    private byte WaitNonZeroStatus (int timeout)
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
	    
	    
	    private byte WaitZeroStatus(int timeout)
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

	    
		private void SetOperatingMode (SensorMode mode)
		{
			unchecked {
				uartDevice.IoCtl ((Int32)UartIOSetConnection, SetupCommand (port, ConnectionType.UART, SensorType.None, mode));
			}
			this.mode = mode;  
	    }
		
		private void ClearPortChanged()
		{
			unchecked {
				uartDevice.IoCtl ((Int32)UartIOClearChanges, SetupCommand (port, ConnectionType.UART, SensorType.None, SensorMode.Mode0));
				uartMemory.Write (UartStatusOffset, new byte[] { (byte)(uartMemory.Read ((int)port, 1) [0] & ~UartPortChanged) });
			}
			this.mode = SensorMode.Mode0;
	    }
		
	    private bool InitUart(SensorMode mode)
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
	            Console.WriteLine("Try to init sensor");
	            //something change wait for it to become ready
	            if (GetConnectionType() != ConnectionType.UART)
	            	return false;
	            ClearPortChanged();
	            System.Threading.Thread.Sleep(InitDelay);
	            status = WaitNonZeroStatus(WaitTimout);
	            if ((status & UartDataReady) != 0 && (status & UartPortChanged) == 0) 
	            {
	                // device ready make sure it is now in the correct mode
	                SetOperatingMode(mode);
	                //status = WaitNonZeroStatus(WaitTimout);
	            }
	        }
	        if ((status & UartDataReady) != 0 && (status & UartPortChanged) == 0)
	            return true;
	        else
	            return false;
	    }
	}
}

