using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonoBrickFirmware.Extensions;
using MonoBrickFirmware.Sensors;

namespace MonoBrickWebServer.Models
{
  	
	public class DummySensor : ISensor
	{
		private SensorPort port;
		private enum DummyMode { Raw = 1, Digital = 2 };
		private DummyMode mode = DummyMode.Raw;
		Random rnd = new Random();
		public DummySensor(SensorPort port)
		{
			this.port = port;
		}

		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
		public string ReadAsString()
		{
			if (mode == DummyMode.Digital)
				return rnd.Next(1).ToString();
			return rnd.Next(1024) + " A/D value";
		}

		/// <summary>
		/// Gets the name of the sensor.
		/// </summary>
		/// <returns>The sensor name.</returns>
		public string GetSensorName()
		{
			return "Dummy Sensor";
		}

		/// <summary>
		/// Selects the next mode.
		/// </summary>
		public void SelectNextMode()
		{
			mode = mode.Next();
		}

		/// <summary>
		/// Selects the previous mode.
		/// </summary>
		public void SelectPreviousMode()
		{
			mode = mode.Previous();
		}

		/// <summary>
		/// Numbers the of modes.
		/// </summary>
		/// <returns>The number of modes</returns>
		public int NumberOfModes()
		{
			return Enum.GetNames(typeof(DummyMode)).Length;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>The mode.</returns>
		public string SelectedMode()
		{
			return mode.ToString();
		}

		public SensorPort Port { get { return port; } }
	}
}