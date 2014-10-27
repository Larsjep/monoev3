using System;
using System.Threading;
using MonoBrickFirmware.Tools;
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
			Update();
		}
	}
  
	public class SensorListner : IDisposable
	{
		private SensorDetector detector = new SensorDetector();
    	private ManualResetEvent terminate = new ManualResetEvent(false);
	  	private bool run = false;
    	Thread thread = null;
		private int interval = 0;
	  	public event Action<ISensor> SensorAttached = delegate {};
		public event Action<SensorPort> SensorDetached = delegate {};
		
		public SensorListner (): this(1000)
		{
		}
		
		public SensorListner (int interval)
		{
			detector.SensorAttached += this.SensorAttached;
			detector.SensorDetached += this.SensorDetached;
			this.interval = interval;
			thread = new Thread(ListenThread);
      		terminate.Reset();
  			run = true;
      		thread.Start();
		}

		private void ListenThread ()
		{
			while (run) 
			{
			    detector.Update();
				terminate.WaitOne(interval);
			}
		}
		
		public void Dispose ()
 		{
			run = false;
			terminate.Set();
			thread.Join();
    	}
	}
}

