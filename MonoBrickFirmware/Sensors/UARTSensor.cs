using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Tools;
using System.Threading;
using System.ComponentModel;

namespace MonoBrickFirmware.Sensors
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
	public abstract class UartSensor:ISensor
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
    	
    	// UART IO control
    	private const UInt32 UartIOReadModeInfo = 0xc03c7501;
		private const UInt32 SensorInfoLength = 56;
    	
    	private const int UartRawDataSize = 32; 
		private const int UartRawBufferLength = 300;
		private const int UartRawBufferSize = UartRawDataSize * UartRawBufferLength;
    	
		
    	private const byte UartDataReady = 8;
		private const byte UartPortChanged = 1;
		
		private UnixDevice UartDevice;
		
		protected const int NumberOfSensorPorts = SensorManager.NumberOfSensorPorts;
		protected SensorPort port;
		protected UARTMode uartMode{get; private set;}

        private int pollTime = 50;
        private EventWaitHandle stopPolling = new ManualResetEvent(false);
        private QueueThread queue = QueueThread.Instance;
        private Thread pollThread = null;
        public UartSensor(SensorPort port, int pollTime = 50)
        {
            this.port = port;
            this.pollTime = pollTime;
            uartMemory = SensorManager.Instance.UartMemory;
            UartDevice = SensorManager.Instance.UartDevice;
            pollThread = new Thread(sensorPollThread);
            pollThread.Start();
        }

        /// <summary>
        /// Stop polling this instance
        /// </summary>
        public void Kill()
        {
            stopPolling.Set();
            pollThread.Join();
        }


        /// <summary>
        /// thread that checks the sensor' state, and raises the propertyChanged event in queue when it is changed.
        /// </summary>
        private void sensorPollThread()
        {
            Thread.CurrentThread.IsBackground = true;
            int lastState = Read();
            while (!stopPolling.WaitOne(pollTime))
            {
                int currenState = Read();
                if (currenState != lastState)
                {
                    queue.Enqueue(propertyChangedEvent, this, new PropertyChangedEventArgs(GetSensorName()));
                    lastState = currenState;
                }
            }
        }

        public event PropertyChangedEventHandler propertyChangedEvent;

        public abstract string ReadAsString();

        public abstract int Read();

        public abstract void SelectNextMode();
		
		public abstract string GetSensorName();
		
		public abstract void SelectPreviousMode();
		
		public abstract int NumberOfModes();
        
        public abstract string SelectedMode();
        
        public SensorPort Port{ get {return port;}}
		
		protected void Reset()
	    {
	        SensorManager.Instance.ResetUart(this.port);
			uartMode = UARTMode.Mode0; 
	        WaitZeroStatus(WaitTimout);
	    }
	    
	    
	    /// <summary>
	    /// Gets the sensor info based on the mode
	    /// </summary>
	    /// <returns>Raw Sensor info data</returns>
		protected byte[] GetSensorInfo()
		{
			ByteArrayCreator command = new ByteArrayCreator ();
			command.Append (new Byte[SensorInfoLength]);
			/*command.Append(new Byte());
			command.Append(new Byte());
			command.Append(new Byte());
			command.Append(new Byte());
			command.Append(new Byte());
			command.Append(new Byte());
			command.Append(new Byte());
			command.Append(new Byte());
			command.Append(new Single());
			command.Append(new Single());
			command.Append(new Single());
			command.Append(new Single());
			command.Append(new Single());
			command.Append(new Single());
			
			
			command.Append(new Int16());
			command.Append(new Int16());
			command.Append(new Byte());
			command.Append (new Byte[5]);
			command.Append(new Int16());*/
			
			command.Append ((byte)this.port);
			command.Append ((byte)this.uartMode);
			byte[] uartData = command.Data;
			unchecked {
				UartDevice.IoCtl ((Int32)UartIOReadModeInfo, uartData);
			}
			return uartData;
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
	        //CheckSensor();
	        return GetRawData(CalcRawOffset(), 1)[0];  
	    }
	    
	    protected byte[] ReadBytes (int length)
		{
			//CheckSensor ();
			return GetRawData (CalcRawOffset(), length);
	    }
		
		
		private byte[] GetRawData (int idx, int length)
		{
			return uartMemory.Read(UartRawOffset + idx,  length);
		}
		
		private int GetActualData ()
		{
			byte[] temp = uartMemory.Read (UartActualOffset, NumberOfSensorPorts * 2);
			return (int) BitConverter.ToInt16(temp,(int) port * 2);
		}
		
		
		private byte GetStatus()
	    {
	        return uartMemory.Read(UartStatusOffset, NumberOfSensorPorts)[(int)port];
	    }
		
		
		private void CheckSensor()
    	{
	        if(SensorManager.Instance.GetConnectionType(this.port) != ConnectionType.UART)
	        	throw new Exception("UART sensor is not connected");
    	}
	    
		private int CalcRawOffset()
    	{
			return (int)port * UartRawBufferSize + GetActualData() * UartRawDataSize;
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
			uartMemory.Write (UartStatusOffset + (int) port, new byte[] { (byte)(uartMemory.Read (UartStatusOffset +(int)port, 1) [0] & ~UartPortChanged) });
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

