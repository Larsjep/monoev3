using System;

namespace MonoBrickFirmware.IO
{
	/// <summary>
	/// Interface for a sensor 
	/// </summary>
    public interface ISensor
	{
		/// <summary>
		/// Initialize this sensor
		/// </summary>
		void Initialize();

		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
        string ReadAsString();
	}
}

