using System;

namespace MonoBrickFirmware.Sensors
{
	public class NoSensor : ISensor
	{
		private SensorPort port;

		public NoSensor (SensorPort port)
		{
			this.port = port;
		}

		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
        public string ReadAsString ()
		{
			return "0";
		}

        
        /// <summary>
        /// Gets the name of the sensor.
        /// </summary>
        /// <returns>The sensor name.</returns>
		public string GetSensorName ()
		{
			return "None";
		}
		
		/// <summary>
		/// Selects the next mode.
		/// </summary>
		public void SelectNextMode ()
		{
			
		}
		
		/// <summary>
		/// Selects the previous mode.
		/// </summary>
		public void SelectPreviousMode ()
		{

		}
		
		/// <summary>
		/// Numbers the of modes.
		/// </summary>
		/// <returns>The number of modes</returns>
		public int NumberOfModes ()
		{
			return 0;
		}
        
        /// <summary>
        /// .m.-,
        /// </summary>
        /// <returns>The mode.</returns>
        public string SelectedMode ()
		{
			return "";
		}
        
        /// <summary>
		/// Sensor port
		/// </summary>
		/// <returns>The sensor port</returns>
		public SensorPort Port {
			get
			{ 
				return port;
			}
    	}

	}
}

