using System;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Sensors;
using System.Threading;

namespace SensorFactoryExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			bool run = true;
			SensorType[] lastSensorType = new SensorType[4];	
			SensorPort[] sensorPort = { SensorPort.In1, SensorPort.In2, SensorPort.In3, SensorPort.In4 };
			ISensor[] sensor = new ISensor[4];
			for (int i = 0; i < 4; i++) {
				lastSensorType [i] = SensorType.None;
				sensor [i] = null;
			}
			ButtonEvents buts = new ButtonEvents ();
			buts.EscapePressed += () => { 
				run = false;
			};
			while (run) {
						
				for (int i = 0; i < 4; i++) {
					SensorType currentType = SensorManager.Instance.GetSensorType (sensorPort [i]);
					if (currentType != lastSensorType [i]) {
						Console.WriteLine (sensorPort [i] + " changed from  " + lastSensorType [i] + " to " + currentType);
						sensor [i] = SensorFactory.GetSensor (sensorPort [i]);
						lastSensorType [i] = currentType;
						Console.WriteLine (sensor [i]);
						if (currentType == SensorType.None) 
						{
							SensorManager.Instance.ResetUart(sensorPort [i]);
							SensorManager.Instance.ResetI2C(sensorPort [i]);
							SensorManager.Instance.SetAnalogMode(AnalogMode.None, sensorPort [i]); 
							
						}
						//sensor[i].GetSensorName();
					}
							
				}
			}
		}
	}
}
