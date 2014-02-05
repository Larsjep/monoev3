using System;
using System.Collections.Generic;

namespace MonoBrickFirmware.Sensors
{
	public class SensorFactory
	{
		public static ISensor GetSensor (SensorPort port)
		{
			ISensor sensor = null;
			if (SensorManager.Instance.GetConnectionType (port) == ConnectionType.UART) {
				System.Threading.Thread.Sleep(1000);
				var temp = new EV3GyroSensor(port);
				System.Threading.Thread.Sleep(1000);
				temp.Read();
				SensorManager.Instance.SetUartOperatingMode(UARTMode.Mode1,port);
			}
			
			SensorType type = SensorManager.Instance.GetSensorType (port);
			Console.WriteLine(type);
			switch (type) {
				case SensorType.Color:
					sensor = new EV3ColorSensor (port); 
					break;
				case SensorType.Gyro:
					sensor = new EV3GyroSensor (port); 
					break;
				case SensorType.IR:
					sensor = new EV3IRSensor (port);
					break;
				case SensorType.NXTColor:
					sensor = new NXTColorSensor (port);
					break;
				case SensorType.NXTLight:
					sensor = new NXTLightSensor (port);
					break;
				case SensorType.NXTSound:
					sensor = new NXTSoundSensor (port);
					break;
				case SensorType.NXTTouch:
					sensor = new NXTTouchSensor (port);
					break;
				case SensorType.NXTUltraSonic:
					sensor = new NXTUltraSonicSensor (port);
					break;
				case SensorType.Touch:
					sensor = new EV3TouchSensor (port);
					break;
				case SensorType.UltraSonic:
					sensor = new EV3UltrasonicSensor (port);
					break;
				case SensorType.NXTI2c:
					var helper = new I2CHelper (port);
					Console.WriteLine(helper.GetSensorName ());
					switch (helper.GetSensorName ()) 
					{
						case "LEGO???Sonar??":
							sensor = new NXTUltraSonicSensor(port);
							Console.WriteLine("Setting sensor values to Ultrasonic");
						break;
						case "HiTechncColor":
							sensor = new HiTecColorSensor(port);
						break;
						case "HiTechncCompass":
							sensor = new HiTecCompassSensor(port);
						break;
						case "HITECHNCAccel.":
							sensor = new HiTecTiltSensor(port);
						break;
					}	
					break;
				case SensorType.I2CUnknown:
						
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
			if(sensor == null)
				Console.WriteLine("Sensor value is null");
			else
				Console.WriteLine("Sensor value is not null");
			return sensor;
		}
	}
	internal class I2CHelper : I2CSensor
	{
		public I2CHelper (SensorPort port) : base (port, 0x02, I2CMode.LowSpeed9V)
		{
			base.Initialise();
		}
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
        public override string ReadAsString ()
		{
			return "";
		}
        
        /// <summary>
        /// Gets the name of the sensor.
        /// </summary>
        /// <returns>The sensor name.</returns>
		public override string GetSensorName ()
		{
			byte[] data = new byte[16];
			var temp = ReadRegister(0x08);
			Buffer.BlockCopy(temp,0,data,0,8);
			System.Threading.Thread.Sleep(50);
			temp = ReadRegister(0x10);
			Buffer.BlockCopy(temp,0,data,8,8);
			return System.Text.Encoding.ASCII.GetString (data);	
		}
		
		/// <summary>
		/// Selects the next mode.
		/// </summary>
		public override void SelectNextMode ()
		{
			return;
		}
		
		/// <summary>
		/// Selects the previous mode.
		/// </summary>
		public override void SelectPreviousMode ()
		{
			return;
		}
		
		/// <summary>
		/// Numbers the of modes.
		/// </summary>
		/// <returns>The number of modes</returns>
		public override int NumberOfModes()
		{
			return 0;
		}
        
        /// <summary>
        /// .m.-,
        /// </summary>
        /// <returns>The mode.</returns>
        public override string SelectedMode ()
		{
			return "";
		}
	}
}

