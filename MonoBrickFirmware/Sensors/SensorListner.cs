using System;
using System.Threading;
using MonoBrickFirmware.Tools;
namespace MonoBrickFirmware.Sensors
{
	public class SensorListner
	{
		private bool run = false;
		private SensorType[] lastSensorType = new SensorType[SensorManager.NumberOfSenosrPorts];	
		private SensorPort[] sensorPort = { SensorPort.In1, SensorPort.In2, SensorPort.In3, SensorPort.In4 };
		private ISensor[] sensor = new ISensor[SensorManager.NumberOfSenosrPorts];
		Thread thread = null;
		public event Action<ISensor> SensorAttached = delegate {};
		public event Action SensorDetached = delegate {};
		public SensorListner ()
		{
			run = false;
			thread = new Thread(ListenThread);
		}
		
		public void Start ()
		{
			if (!run) 
			{
				for (int i = 0; i < SensorManager.NumberOfSenosrPorts; i++) {
					lastSensorType [i] = SensorType.None;
					sensor [i] = null;
				}
				run = true;
				thread.Start ();
			}
		}
		
		public void Stop ()
		{
			if (run) 
			{
				run = false;
				thread.Join();
			}
		}
		
		public bool IsListning{ get {return run;}}
		
		private void ListenThread ()
		{
			while (run) 
			{
				for (int i = 0; i < SensorManager.NumberOfSenosrPorts; i++) {
					SensorType currentType = SensorManager.Instance.GetSensorType (sensorPort [i]);
					if (currentType != lastSensorType [i]) {
							
						//Console.WriteLine (sensorPort [i] + " changed from  " + lastSensorType [i] + " to " + currentType);
						sensor [i] = SensorFactory.GetSensor (sensorPort [i]);
						lastSensorType [i] = currentType;
						if (currentType == SensorType.None) {
							SensorManager.Instance.ResetUart (sensorPort [i]);
							SensorManager.Instance.ResetI2C (sensorPort [i]);
							SensorManager.Instance.SetAnalogMode (AnalogMode.None, sensorPort [i]); 
							SensorAttached (sensor [i]);	
						} else {
							SensorDetached ();
						}
					}
				}
			}	
		
		}
		
		  
		
	}
}

