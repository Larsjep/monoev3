using System;
using System.Collections.Generic;
using MonoBrickFirmware.Extensions;

namespace MonoBrickFirmware.Sensors
{
	public class SensorFactory
	{
		public static ISensor GetSensor (SensorPort port)
		{
			ISensor sensor = null;
			
			SensorType sensorType = SensorManager.Instance.GetSensorType (port);
			ConnectionType connectionType = SensorManager.Instance.GetConnectionType (port);
			switch (sensorType) {
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
					sensor = helper.GetSensor ();
					break;
				case SensorType.Unknown:
					if (connectionType == ConnectionType.UART) {
						var uartHelper = new UARTHelper (port);
						sensor = uartHelper.GetSensor ();
					}
					if (connectionType == ConnectionType.InputResistor) {
						sensor  = new EV3TouchSensor(port);
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
				case SensorType.None:
					
					break;
			}
			return sensor;
		}
	}
	
	internal class UARTHelper : UartSensor{
		private const UInt32 SensorNameLength = 12;
    	private static byte[] IRName = {0x49, 0x52, 0x2d, 0x50, 0x52, 0x4f, 0x58, 0x00, 0x00, 0x00, 0x00, 0x00};
    	private static byte[] ColorName = {0x43, 0x4f, 0x4c, 0x2d, 0x52, 0x45, 0x46, 0x4c, 0x45, 0x43, 0x54, 0x00};
    	private static byte[] GyroName = {0x47, 0x59, 0x52, 0x4f, 0x2d, 0x41, 0x4e, 0x47, 0x00, 0x00, 0x00, 0x00};
    	private static byte[] UltrasonicName = {0x55, 0x53, 0x2d, 0x44, 0x49, 0x53, 0x54, 0x2d, 0x43, 0x4d, 0x00, 0x00};
    	private Dictionary<byte[], ISensor> sensorDictionary = null;

		public UARTHelper (SensorPort port) : base (port)
		{
			base.Initialise(base.uartMode);
			base.SetMode(UARTMode.Mode0);
			sensorDictionary = new Dictionary<byte[], ISensor>();
			sensorDictionary.Add(IRName, new EV3IRSensor(port));
			sensorDictionary.Add(GyroName, new EV3GyroSensor(port));
			sensorDictionary.Add(ColorName, new EV3ColorSensor(port));
			sensorDictionary.Add(UltrasonicName, new EV3UltrasonicSensor(port));
			System.Threading.Thread.Sleep(100);
		}
		
		private bool ByteArrayCompare(byte[] a1, byte[] a2) 
		{
		  for(int i=0; i<a1.Length; i++)
		    if(a1[i]!=a2[i])
		      return false;
		  return true;
		}
		
		public ISensor GetSensor ()
		{
			byte[] data = new byte[SensorNameLength];
			data[0] = 0;
			while(data[0] == 0){
				data = this.GetSensorInfo ();
			}
			byte[] name = new byte[SensorNameLength];
			Array.Copy(data, name, SensorNameLength);
			foreach (KeyValuePair<byte[], ISensor> pair in sensorDictionary) {
				if (ByteArrayCompare (pair.Key, name)) {
					return pair.Value;
				}
			}
			return null;
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
			return "";
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
	
	internal class I2CHelper : I2CSensor
	{
		private Dictionary<byte[], ISensor> sensorDictionary = null;
    	
    	private static byte[] SonarName = {0x4c, 0x45, 0x47, 0x4f, 0x00, 0xff, 0xff, 0xff, 0x53, 0x6f, 0x6e, 0x61, 0x72, 0x00, 0xff, 0xff};
    	private static byte[] HTColorName = {0x48, 0x69, 0x54, 0x65, 0x63, 0x68, 0x6e, 0x63, 0x43, 0x6f, 0x6c, 0x6f, 0x72, 0x20, 0x20, 0x20};
    	private static byte[] HTCompassName = {0x48, 0x69, 0x54, 0x65, 0x63, 0x68, 0x6e, 0x63, 0x43, 0x6f, 0x6d, 0x70, 0x61, 0x73, 0x73, 0x20};
    	private static byte[] HTAccelName = {0x48, 0x49, 0x54, 0x45, 0x43, 0x48, 0x4e, 0x43, 0x41, 0x63, 0x63, 0x65, 0x6c, 0x2e, 0x20, 0x20};
		
		public I2CHelper (SensorPort port) : base (port, 0x02, I2CMode.LowSpeed9V)
		{
			base.Initialise();
			sensorDictionary = new Dictionary<byte[], ISensor>();
			sensorDictionary.Add(SonarName , new NXTUltraSonicSensor(port));
			sensorDictionary.Add(HTColorName, new HiTecColorSensor(port));
			sensorDictionary.Add(HTCompassName, new HiTecCompassSensor(port));
			sensorDictionary.Add(HTAccelName, new HiTecTiltSensor(port));
		}
		
		private bool ByteArrayCompare(byte[] a1, byte[] a2) 
		{
		  for(int i=0; i<a1.Length; i++)
		    if(a1[i]!=a2[i])
		      return false;
		  return true;
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
		
		public ISensor GetSensor ()
		{
			byte[] data = new byte[16];
			var temp = ReadRegister (0x08);
			Buffer.BlockCopy (temp, 0, data, 0, 8);
			System.Threading.Thread.Sleep (100);
			temp = ReadRegister (0x10);
			Buffer.BlockCopy (temp, 0, data, 8, 8);
			foreach (KeyValuePair<byte[], ISensor> pair in sensorDictionary) {
				if (ByteArrayCompare (pair.Key, data)) {
					return pair.Value;
				}
			}
			return null;
		}
        
        /// <summary>
        /// Gets the name of the sensor.
        /// </summary>
        /// <returns>The sensor name.</returns>
		public override string GetSensorName ()
		{
			return "";
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
	
	internal class DummySensor : ISensor
	{
		private SensorPort port;
		private enum DummyMode{Raw = 1, Digital = 2};
		private DummyMode mode = DummyMode.Raw;
		Random rnd = new Random();
		public DummySensor(SensorPort port)
		{
			this.port = port;
			System.Threading.Thread.Sleep(500);
		}
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
        public string ReadAsString ()
		{
			if (mode == DummyMode.Digital)
				return rnd.Next(1).ToString(); 
			return rnd.Next(1024) + " A/D value";
		}
		
		/// <summary>
        /// Gets the name of the sensor.
        /// </summary>
        /// <returns>The sensor name.</returns>
		public string GetSensorName ()
		{
			return "Dummy Sensor";
		}
		
		/// <summary>
		/// Selects the next mode.
		/// </summary>
		public void SelectNextMode ()
		{
			mode = mode.Next();
		}
		
		/// <summary>
		/// Selects the previous mode.
		/// </summary>
		public void SelectPreviousMode ()
		{
			mode = mode.Previous();
		}
		
		/// <summary>
		/// Numbers the of modes.
		/// </summary>
		/// <returns>The number of modes</returns>
		public int NumberOfModes()
		{
			return Enum.GetNames(typeof(DummyMode)).Length;
		}
        
        /// <summary>
        /// .m.-,
        /// </summary>
        /// <returns>The mode.</returns>
        public string SelectedMode ()
		{
			return mode.ToString();
		}
		
		public SensorPort Port{ get {return port;}}
	}
	
}

