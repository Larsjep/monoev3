using System;
namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Interface for a sensor 
	/// </summary>
    public interface ISensor
	{
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
        string ReadAsString();
        
        /// <summary>
        /// Gets the name of the sensor.
        /// </summary>
        /// <returns>The sensor name.</returns>
		string GetSensorName();
		
		/// <summary>
		/// Selects the next mode.
		/// </summary>
		void SelectNextMode();
		
		/// <summary>
		/// Selects the previous mode.
		/// </summary>
		void SelectPreviousMode();
		
		/// <summary>
		/// Numbers the of modes.
		/// </summary>
		/// <returns>The number of modes</returns>
		int NumberOfModes();
        
        /// <summary>
        /// .m.-,
        /// </summary>
        /// <returns>The mode.</returns>
        string SelectedMode();
        
	}
}

