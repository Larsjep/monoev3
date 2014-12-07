using System;
using System.Collections.Generic;
using MonoBrickFirmware.Sensors;
namespace MonoBrickWebServer.Models
{
	public class EV3Model
	{
		private SensorDetector detector = null;
		public EV3Model (bool useDummy)
		{
			Motors = new MotorModelList (useDummy);
			Sensors = new SensorModelList (useDummy);
			if (!useDummy) 
			{
				LCD = new LcdModel ();
				detector = new SensorDetector ();
				detector.SensorAttached += HandleSensorAttached;
				detector.SensorDetached += HandleSensorDetached;
			} 
			else 
			{
				LCD = new DummyLcdModel();
			}
		}
		public MotorModelList Motors{ get; private set;}
		public SensorModelList Sensors{ get; private set;}
		public ILcdModel LCD{get; private set;}
		public void Update()
		{
			if (detector != null) 
			{
				detector.Update();
			}
			Motors.Update ();
			Sensors.Update ();
		}

		private void HandleSensorAttached (ISensor sensor)
		{
			Sensors [sensor.Port].AttachSensor (sensor);
		}

		private void HandleSensorDetached (SensorPort port)
		{
			Sensors[port].DetachSensor ();
		}

	}
}

