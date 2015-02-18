using System;

namespace MonoBrickFirmware.Sensors
{
	public class SensorDetector
	{
		private SensorType[] lastSensorType = new SensorType[SensorManager.NumberOfSensorPorts];
		private SensorPort[] sensorPort = { SensorPort.In1, SensorPort.In2, SensorPort.In3, SensorPort.In4 };
		private ISensor[] sensor = new ISensor[SensorManager.NumberOfSensorPorts];
		private bool hasBeenInitialized = false;
		public event Action<ISensor> SensorAttached = delegate { };
		public event Action<SensorPort> SensorDetached = delegate { };

		public void Update()
		{
			if (!hasBeenInitialized)
			{
				Initialize();
				hasBeenInitialized = true;
			}
			for (int i = 0; i < SensorManager.NumberOfSensorPorts; i++)
			{
				SensorType currentType = SensorManager.Instance.GetSensorType(sensorPort[i]);
				if (currentType != lastSensorType[i])
				{
					sensor[i] = SensorFactory.GetSensor(sensorPort[i]);
					lastSensorType[i] = currentType;
					if (currentType == SensorType.None)
					{
						ResetSensor(sensorPort[i]);
						SensorDetached(sensorPort[i]);
					}
					else
					{
						SensorAttached(sensor[i]);
					}
				}
			}
		}

		private void ResetSensor(SensorPort port)
		{
			SensorManager.Instance.ResetUart(port);
			SensorManager.Instance.ResetI2C(port);
			SensorManager.Instance.SetAnalogMode(AnalogMode.None, port);
		}

		private void Initialize()
		{
			for (int i = 0; i < SensorManager.NumberOfSensorPorts; i++)
			{
				lastSensorType[i] = SensorType.None;
				ResetSensor(sensorPort[i]);
				sensor[i] = null;
			}
		}
	}
}

