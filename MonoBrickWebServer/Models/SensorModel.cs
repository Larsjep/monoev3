using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonoBrickFirmware.Sensors;

namespace MonoBrickWebServer.Models
{
	public class SensorModel : ISensorModel
	{
		private ISensor sensor;
		private bool isDummy;
		private bool updateMode = true;
		private bool updateName = true;
		public SensorModel(MonoBrickFirmware.Sensors.SensorPort port, bool isDummy)
		{
			Port = port.ToString();
			this.isDummy = isDummy;
			if (isDummy) 
			{
				sensor = new DummySensor(port);
			} 
			else 
			{
				sensor = new NoSensor (port);
			}
		}

		public void AttachSensor (ISensor sensor)
		{
			if (isDummy) 
			{
				sensor = new DummySensor (sensor.Port);
			} 
			else 
			{
				this.sensor = sensor;
			}
			updateMode = true;
			updateName = true;

		}

		public void DetachSensor ()
		{
			if (isDummy) 
			{
				sensor = new DummySensor (sensor.Port);
			} 
			else 
			{
				this.sensor = new NoSensor(sensor.Port);
			}
			updateMode = true;
			updateName = true;
		}

		public void NextMode()
		{
			sensor.SelectNextMode();
			updateMode = true;
		}

		public void PreviousMode()
		{
			sensor.SelectPreviousMode();
			updateMode = true;
		}

		public void Update()
		{
			if (updateMode)
			{
				Mode = sensor.SelectedMode();
				updateMode = false;
			}
			if (updateName)
			{
				Name = sensor.GetSensorName();
				updateName = false;
			}
			Value = sensor.ReadAsString();
		}

		public string Mode  { get; private set; }
		public string Port  { get; private set; }
		public string Name  { get; private set; }
		public string Value { get; private set; }
	}
}