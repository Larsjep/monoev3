using System;
using System.Threading;

namespace MonoBrickFirmware.Sensors
{
	public class SensorAttachment
	{
		private static int PollInterval = 500;

		/// <summary>
		/// Wait for a sensor on any port.
		/// </summary>
		/// <typeparam name="SensorType">The sensor type to wait for</typeparam>
		public static SensorType Wait<SensorType>()
			where SensorType : ISensor
		{
			return WaitForSensor<SensorType>(null, new CancellationToken(false));
		}

		/// <summary>
		/// Wait for a sensor on any port with ability to cancel.
		/// </summary>
		/// <param name="token">Token to cancel</param>
		/// <typeparam name="SensorType">The sensor type to wait for</typeparam>
		public static SensorType Wait<SensorType>(CancellationToken token)
			where SensorType : ISensor
		{
			return WaitForSensor<SensorType>(null, token);
		}


		/// <summary>
		/// Wait for a sensor on a specified port.
		/// </summary>
		/// <param name="port">Port sensor should be attached to.</param>
		/// <typeparam name="SensorType">The sensor type to wait for</typeparam>
		public static SensorType Wait<SensorType>(SensorPort port)
			where SensorType : ISensor
		{
			return WaitForSensor<SensorType>(port, new CancellationToken(false));
		}

		/// <summary>
		/// Wait the specified port and token with ability to cancel.
		/// </summary>
		/// <param name="port">Port sensor should be attached to.</param>
		/// <param name="token">Token to cancel</param>
		/// <typeparam name="SensorType">The sensor type to wait for</typeparam>
		public static SensorType Wait<SensorType>(SensorPort port, CancellationToken token)
			where SensorType : ISensor
		{
			return WaitForSensor<SensorType>(port, token);
		}


		private static SensorType WaitForSensor<SensorType>(SensorPort? port, CancellationToken token)
			where SensorType : ISensor
		{
			ManualResetEvent sensorAttached = new ManualResetEvent (false);
			SensorDetector detector = new SensorDetector ();
			bool run = true;
			bool checkForPort = port != null;
			SensorType newSensor = default(SensorType);

			detector.SensorAttached += delegate(ISensor sensor) {
				if (sensor.GetType () == typeof(SensorType)) {
					if (!checkForPort || sensor.Port == port.Value) {
						newSensor = (SensorType)sensor;
						sensorAttached.Set ();
						run = false;
					}
				}		
			};
			WaitHandle[] handles = new WaitHandle[]{sensorAttached, token.WaitHandle};
			while (run) {
				detector.Update ();
				if (token.IsCancellationRequested) 
				{
					run = false;
				}
				WaitHandle.WaitAny(handles, PollInterval);
			}
			return newSensor;
		}


	}
}

