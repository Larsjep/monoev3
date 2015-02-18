using System;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Class for the Mindsensor glideWheel angle sensor
	/// </summary>
	public class MSAngleSensor : I2CSensor
	{
		private enum GlideWheelRegister : byte
		{
			Command = 0x41, Angle = 0x42, RAW = 0x46, RPM = 0x4A
		};

		private static byte ResetCommand = (byte)'r';

		public MSAngleSensor (SensorPort Port) : base (Port, (byte)0x30, I2CMode.LowSpeed)
		{
			base.Initialise();
			base.Initialise();
		}

		public void ResetAngle()
		{
			WriteRegister((byte)GlideWheelRegister.Command, (byte)ResetCommand);
		}

		public int ReadAngle ()
		{
			return (int) BitConverter.ToInt32(ReadRegister((byte)GlideWheelRegister.Angle, 4),0);
		}

		public int ReadRAW ()
		{
			return (int) BitConverter.ToInt32(ReadRegister((byte)GlideWheelRegister.RAW, 4),0);
		}

		public int ReadRPM ()
		{
			return (int) BitConverter.ToInt16(ReadRegister((byte)GlideWheelRegister.RPM, 2),0);
		}

		public override string ReadAsString()
		{
			return("Angle: " + ReadAngle() + " RPM: " + ReadRPM());
		}

		public override string GetSensorName()
		{
			return "Mindsensors Angle";
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
			return ("Angle mode");
		}
	}
}

