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
			LCD = new LcdModel(useDummy);
			if (!useDummy) 
			{
				detector = new SensorDetector ();
				detector.SensorAttached += HandleSensorAttached;
				detector.SensorDetached += HandleSensorDetached;
			}
		}
		public MotorModelList Motors{ get; private set;}
		public SensorModelList Sensors{ get; private set;}
		public LcdModel LCD{get; private set;}
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

