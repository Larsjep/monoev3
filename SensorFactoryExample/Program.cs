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
				sensor[i] = null;
			}
			ButtonEvents buts = new ButtonEvents ();
			buts.EscapePressed += () => { 
				run = false;
			};
			while (run) {
						
				for (int i = 0; i < 4; i++) {
					SensorType currentType = SensorManager.Instance.GetSensorType (sensorPort [i]);
					Console.WriteLine (currentType);
					ConnectionType connectionType = SensorManager.Instance.GetConnectionType(sensorPort[i]);
					Console.WriteLine(connectionType);
					sensor[i] = SensorFactory.GetSensor(sensorPort[i]);
						
					if (currentType != lastSensorType [i]) {
						connectionType = SensorManager.Instance.GetConnectionType(sensorPort[i]);
						//Console.WriteLine(connectionType);
						//Console.WriteLine (sensorPort [i] + " changed from  " + lastSensorType [i] + " to " + currentType);
						lastSensorType [i] = currentType;
						//Console.WriteLine(sensor[i]);
						//sensor[i].GetSensorName();
					}
							
				}
			}
		}
	}
}
