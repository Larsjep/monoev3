using System;

namespace MonoBrickFirmware.Sensors
{
	
	public enum Range{Short = 1, Medium = 2, Long = 3}


	/// <summary>
	/// Class for Mindsensors high precision infrared distance sensor
	/// </summary>
	public class MSDistanceSensor :I2CSensor
	{
		private static byte PowerOnCommand = (byte)'E';
		private static byte PowerOffCommand = (byte)'D';
		private Range? range = null;

		private enum DistanceRegister : byte
		{
			Command = 0x41, DistanceLsb = 0x42, DistanceMsb = 0x43, VoltageLsb = 0x44, VoltageMsb = 0x45
		};


		public MSDistanceSensor (SensorPort port) : base(port, (byte)0x02, I2CMode.LowSpeed9V)
		{	
			base.Initialise();
		}


		/// <summary>
		/// Gets the range type of the sensor.
		/// </summary>
		/// <returns>The range.</returns>
		public Range GetRange ()
		{
			string deviceId = base.GetDeviceId ();
			Console.WriteLine("Length: " + deviceId.Length);
			if (!range.HasValue) 
			{ 
				switch (deviceId) {
				case "DIST-S":
					range = Range.Short;
					break;	
				case "DIST-M":
					range = Range.Medium;
					break;	
				case "DIST-L":
					range = Range.Long;
					break;	
				}
			}
			return range.Value;		
		}

		/// <summary>
		/// Gets the distance in mm.
		/// </summary>
		/// <returns>The distance in mm.</returns>
		public int GetDistance()
		{
			return (int) BitConverter.ToInt16(ReadRegister((byte)DistanceRegister.DistanceLsb, 2),0);
		}

		/// <summary>
		/// Gets the volgage consumption in mA.
		/// </summary>
		/// <returns>The volgage consumption in mA.</returns>
		public int GetVolgage ()
		{
			return (int) BitConverter.ToInt16(ReadRegister((byte)DistanceRegister.DistanceLsb, 2),0);
		}

		/// <summary>
		/// Powers the sensor on.
		/// </summary>
		public void PowerOn()
		{
			WriteRegister((byte)DistanceRegister.Command, (byte)PowerOnCommand);
		}

		/// <summary>
		/// Powers the sensor off.
		/// </summary>
		public void PowerOff()
		{
			WriteRegister((byte)DistanceRegister.Command, (byte)PowerOffCommand);

		}

		public override string ReadAsString()
		{
			return("Distance: " + GetDistance() + " mm");
		}

		public override string GetSensorName()
		{
			return "Mindsensors " + GetRange() + " range distance";
		}

		public override int NumberOfModes()
		{
			return 1;
		}

		public override void SelectNextMode()
		{
			return;
		}

		public override void SelectPreviousMode()
		{
			return;
		}

		public override string SelectedMode()
		{
			return ("Distance mode");
		}
	}
}

