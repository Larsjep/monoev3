using System;

namespace MonoBrickFirmware.IO
{
	/// <summary>
	/// UART sensor. Should not be used for inheritance
	/// </summary>
	public sealed class UARTSensor : UartSensorAbstraction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.UARTSensor"/> class.
		/// </summary>
		/// <param name="port">Sensor port to use</param>
		public UARTSensor (SensorPort port): this(port, UARTMode.Mode0)  
		{
				
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.UARTSensor"/> class.
		/// </summary>
		/// <param name="port">Sensor port to use.</param>
		/// <param name="mode">UART mode to use</param>
		public UARTSensor (SensorPort port, UARTMode mode): base(port)
		{
			base.Initialise(mode);
			base.Initialise(mode);//Do it two times to make it work
				
		}
		
		
		/// <summary>
		/// Sets the UART mode.
		/// </summary>
		/// <returns><c>true</c>, if mode was set, <c>false</c> otherwise.</returns>
		/// <param name="mode">Mode to set</param>
		public new bool SetMode(UARTMode mode)
	    {
	       base.SetMode(mode); 
	    }
	   	
	   	/// <summary>
	   	/// Reads a byte.
	   	/// </summary>
	   	/// <returns>The byte that was read.</returns>
	   	public new byte ReadByte()
	    {
	       return base.ReadByte();  
	    }
	    
	    /// <summary>
	    /// Reads an array of bytes.
	    /// </summary>
	    /// <returns>The bytes that was read.</returns>
	    /// <param name="length">Length to read.</param>
	    public new byte[] ReadBytes (int length)
		{
			return base.ReadBytes(length);
	    }
		
	}
}

