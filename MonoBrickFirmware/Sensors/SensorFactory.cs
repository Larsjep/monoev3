using System;

namespace MonoBrickFirmware.Sensors
{
	public class SensorFactory
	{
		public static ISensor GetSensor (SensorPort port)
		{
			ISensor sensor = null;
			SensorType type = SensorManager.Instance.GetSensorType (port);
			switch (type) 
			{
				case SensorType.Color:
					sensor = new EV3ColorSensor(port); 
				break;
				case SensorType.Gyro:
					sensor = new EV3GyroSensor(port); 
				break;
				case SensorType.IR:
					sensor = new EV3IRSensor(port);
				break;
				case SensorType.NXTColor:
					sensor = new NXTColorSensor(port);
				break;
				case SensorType.NXTLight:
					sensor = new NXTLightSensor(port);
				break;
				case SensorType.NXTSound:
					sensor = new NXTLightSensor(port);
				break;
				case SensorType.NXTTouch:
					sensor = new NXTTouchSensor(port);
				break;
				case SensorType.NXTUltraSonic:
					sensor = new NXTUltraSonicSensor(port);
				break;
				case SensorType.Touch:
					sensor = new EV3TouchSensor(port);
				break;
				case SensorType.UltraSonic:
					sensor = new EV3UltrasonicSensor(port);
				break;
				case SensorType.I2CUnknown:
					
				break;
				case SensorType.NXTI2c:
				
				break;
				case SensorType.NXTTemperature:
					
				break;
				case SensorType.LMotor:
					
				break;
				case SensorType.MMotor:
					
				break;
				case SensorType.NXTTest:
					
				break;
				case SensorType.Terminal:
					
				break;
				case SensorType.Test:
					
				break;
				case SensorType.Error:
				
				break;
				case SensorType.Unknown:
					
				break;
			}
			return sensor;
		}
	}
}

