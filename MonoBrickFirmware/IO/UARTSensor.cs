using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.IO
{
	/// <summary>
	/// Sensor modes
	/// </summary>
	public enum UARTMode {
		#pragma warning disable 
		Mode0 = 0, Mode1 = 1, Mode2 = 2, Mode3 = 3, Mode4 = 4, Mode5 = 5, Mode6 = 6, Mode7 = 7	
		#pragma warning restore
	};
	
	/// <summary>
	/// Class for reading and writing data to a UART sensor
	/// </summary>
	public abstract class UartSensor
	{
		private MemoryArea uartMemory;
		
		private const int WaitTimout = 100;
		private const int WaitFrequency = 100;
		private const int SetUpRetry = 100;
		private const int InitDelay = 5;
		private const int InitRetry = 100;
		
		
		//UART offset
		private const int UartStatusOffset = 42608;
    	private const int UartActualOffset = 42592;
    	private const int UartRawOffset = 4192;
    	
    	private const int UartRawDataSize = 32; 
		private const int UartRawBufferLength = 300;
		private const int UartRawBufferSize = UartRawDataSize * UartRawBufferLength;
    	
		
    	private const byte UartDataReady = 8;
		private const byte UartPortChanged = 1;
		
		
		protected const int NumberOfSenosrPorts = SensorManager.NumberOfSenosrPorts;
		protected SensorPort port;
		protected UARTMode uartMode{get; private set;}
		
		public UartSensor (SensorPort port)
		{
			this.port = port;
			uartMemory = SensorManager.Instance.UartMemory;
		}
		
		protected void Reset()
	    {
	        SensorManager.Instance.ResetUart(this.port);
			uartMode = UARTMode.Mode0; 
	        WaitZeroStatus(WaitTimout);
	    }
		
		
	    protected bool Initialise(UARTMode mode)
	    {
	        for(int i = 0; i < InitRetry; i++)
	        {
	            if (SensorManager.Instance.GetConnectionType(this.port) != ConnectionType.UART)
	                return false;
	            if (InitUart(mode))
	            {
	                return true;
	            }
	            Reset();
	        }
	        return false;
	    }
	    
		protected bool SetMode(UARTMode mode)
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
	        return GetRawData(CalcRawOffset(), 1)[0];  
	    }
	    
	    protected byte[] ReadBytes (int length)
		{
			CheckSensor ();
			return GetRawData (CalcRawOffset(), length);
	    }
		
		
		private byte[] GetRawData (int idx, int length)
		{
			return uartMemory.Read(UartRawOffset + idx,  length);
		}
		
		private byte[] GetActualData ()
		{
			return uartMemory.Read(UartActualOffset, NumberOfSenosrPorts * 2); 
		}
		
		
		private byte GetStatus()
	    {
	        return uartMemory.Read(UartStatusOffset, NumberOfSenosrPorts)[(int)port];
	    }
		
		
		private void CheckSensor()
    	{
	        if(SensorManager.Instance.GetConnectionType(this.port) != ConnectionType.UART)
	        	throw new Exception("UART sensor is not connected");
    	}
	    
		private int CalcRawOffset()
    	{
        	return  (int)port * UartRawBufferSize + GetActualData()[(int) port * 2] * UartRawDataSize;
    	}
    	
	    private byte WaitNonZeroStatus (int timeout)
		{
			int target = timeout/WaitFrequency;
			int time = 0;
	        byte status = 0;
			while (time < target) {
				System.Threading.Thread.Sleep(WaitFrequency);
				status = GetStatus();
				if(status != 0 ||  SensorManager.Instance.GetConnectionType(this.port) != ConnectionType.UART)
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
	            if ( SensorManager.Instance.GetConnectionType(this.port) !=  ConnectionType.UART)
	                return status;
	            System.Threading.Thread.Sleep(WaitFrequency);
	            timeout = timeout + WaitFrequency;
	            status = GetStatus();
	        }
	        return status;       
	    }

	    
		private void SetOperatingMode (UARTMode mode)
		{
			SensorManager.Instance.SetUartOperatingMode(mode, this.port);
			this.uartMode = mode;  
	    }
		
		private void ClearPortChanged()
		{
			SensorManager.Instance.ClearUartPortChanged(this.port);
			uartMemory.Write (UartStatusOffset, new byte[] { (byte)(uartMemory.Read ((int)port, 1) [0] & ~UartPortChanged) });
			this.uartMode = UARTMode.Mode0;
	    }
		
	    private bool InitUart (UARTMode mode)
		{
			for (int i = 0; i <  SetUpRetry; i++) {
				byte status;
				if ( SensorManager.Instance.GetConnectionType (this.port) != ConnectionType.UART)
					return false;
				// now try and configure as a UART
	        
				SetOperatingMode (mode);
				status = WaitNonZeroStatus (WaitTimout);
				while ((status & UartPortChanged) != 0) {
					//something change wait for it to become ready
					if ( SensorManager.Instance.GetConnectionType (this.port) != ConnectionType.UART)
						return false;
					ClearPortChanged ();
					System.Threading.Thread.Sleep (InitDelay);
					status = WaitNonZeroStatus (WaitTimout);
					if ((status & UartDataReady) != 0 && (status & UartPortChanged) == 0) {
						// device ready make sure it is now in the correct mode
						SetOperatingMode (mode);
						//status = WaitNonZeroStatus(WaitTimout);
					}
				}
				if ((status & UartDataReady) != 0 && (status & UartPortChanged) == 0)
					return true;
			}
			return false;
	    }
	}
}

