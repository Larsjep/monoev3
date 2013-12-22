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
	}
}

