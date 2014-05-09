using System;
using System.Threading;
using MonoBrickFirmware.Tools;
namespace MonoBrickFirmware.Sensors
{
	public class SensorListner : IDisposable
	{
		private bool run = false;
		private SensorType[] lastSensorType = new SensorType[SensorManager.NumberOfSensorPorts];	
		private SensorPort[] sensorPort = { SensorPort.In1, SensorPort.In2, SensorPort.In3, SensorPort.In4 };
		private ISensor[] sensor = new ISensor[SensorManager.NumberOfSensorPorts];
		Thread thread = null;
		private int interval = 0;
		public event Action<ISensor> SensorAttached = delegate {};
		public event Action<SensorPort> SensorDetached = delegate {};
		
		public SensorListner (): this(1000)
		{
		
		
		}
		
		private SensorListner (int interval)
		{
			this.interval = interval;
			run = false;
			thread = new Thread(ListenThread);
			Start();
		}
		
		private void Start ()
		{
			if (!run) 
			{
				for (int i = 0; i < SensorManager.NumberOfSensorPorts; i++) {
					lastSensorType [i] = SensorType.None;
					SensorManager.Instance.ResetUart (sensorPort [i]);
					SensorManager.Instance.ResetI2C (sensorPort [i]);
					SensorManager.Instance.SetAnalogMode (AnalogMode.None, sensorPort [i]); 
					sensor [i] = null;
				}
				run = true;
				thread.Start ();
			}
		}
		
		public void Kill ()
		{
			if (run) 
			{
				run = false;
				thread.Join();
			}
		}
		
		private bool IsListning{ get {return run;}}
		
		private void ListenThread ()
		{
			while (run) {
				for (int i = 0; i < SensorManager.NumberOfSensorPorts; i++) {
					SensorType currentType = SensorManager.Instance.GetSensorType (sensorPort [i]);
					if (currentType != lastSensorType [i]) {
						//Console.WriteLine (sensorPort [i] + " changed from  " + lastSensorType [i] + " to " + currentType);
						lock (this) 
						{
							sensor [i] = SensorFactory.GetSensor (sensorPort [i]);
							lastSensorType [i] = currentType;
						}
						if (currentType == SensorType.None) {
							SensorManager.Instance.ResetUart (sensorPort [i]);
							SensorManager.Instance.ResetI2C (sensorPort [i]);
							SensorManager.Instance.SetAnalogMode (AnalogMode.None, sensorPort [i]); 
							SensorDetached (sensorPort [i]);	
						} else {
							SensorAttached (sensor [i]);
						}
					}
				}
				System.Threading.Thread.Sleep(interval);
			}	
		
		}
		
		public void Dispose ()
 		{
			Kill();
 		}
		  
		
	}
}

