using System;
using System.Threading;

namespace MonoBrickFirmware.Sensors
{
	public class SensorAttachment
	{
		private static int PollInterval = 500;
		/// <summary>
		/// Wait for a sensor on a specified port.
		/// </summary>
		/// <param name="port">Port the sensor should be acctached to</param>
		/// <typeparam name="SensorType">The sensor type to wait for</typeparam>
		public static SensorType Wait<SensorType> (SensorPort port)
			where SensorType : ISensor
		{
			ManualResetEvent sensorAttached = new ManualResetEvent(false);
			SensorDetector detector = new SensorDetector();
			bool run = true;
			SensorType newSensor = default(SensorType);
			detector.SensorAttached += delegate(ISensor sensor) 
			{
				if(sensor.Port == port && sensor.GetType() == typeof(SensorType))
				{
					newSensor = (SensorType)sensor;
					sensorAttached.Set();
					run = false;
				}		
			};
			while (run) 
			{
		    	detector.Update();
				sensorAttached.WaitOne(PollInterval);
			}
			return newSensor;
		}

		/// <summary>
		/// Wait for a sensor on any port.
		/// </summary>
		/// <typeparam name="SensorType">The sensor type to wait for</typeparam>
		public static SensorType Wait<SensorType>()
			where SensorType : ISensor
		{
			ManualResetEvent sensorAttached = new ManualResetEvent(false);
			SensorDetector detector = new SensorDetector();
			bool run = true;
			SensorType newSensor = default(SensorType);
			detector.SensorAttached += delegate(ISensor sensor) 
			{
				if(sensor.GetType() == typeof(SensorType))
				{
					sensorAttached.Set();
					newSensor = (SensorType)sensor;
					run = false;
				}		
			};
			while (run) 
			{
		    	detector.Update();
				sensorAttached.WaitOne(PollInterval);
			}
			sensorAttached.WaitOne();
			return newSensor;
		}


	}
}

