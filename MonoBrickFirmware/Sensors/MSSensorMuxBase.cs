using System;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Port of the MUX
	/// </summary>
	public enum MSSensorMUXPort : byte{
		C1 = (byte) 0xA0,
		C2 = (byte) 0xA2,
		C3 = (byte) 0xA4
	};

	/// <summary>
	/// Possible modes of the sensors.
	/// </summary>
	public enum MSSensorMUXMode : byte{
		Mode0 = (byte) 0,
		Mode1 = (byte) 1,
		Mode2 = (byte) 2,
		//for the Touch Sensor, the mode (on register 0x52) needs to be set to 15
		TouchMode = (byte) 15
	};

	/// <summary>
	/// Base for every Sensor attached to the MUX.
	/// </summary>
	public class MSSensorMUXBase : I2CSensor
	{
		private byte Mode = (byte) 0;
		private byte NumberOfM = (byte) 1;

		public MSSensorMUXBase (SensorPort Port, MSSensorMUXPort Address) : base (Port, (byte)Address, I2CMode.LowSpeed)
		{
			base.Initialise ();
			SetSensorMode ();
			NumberOfM = 2;
		}

		public MSSensorMUXBase (SensorPort Port, MSSensorMUXPort Address, MSSensorMUXMode SensorMode) : base (Port, (byte)Address, I2CMode.LowSpeed)
		{
			base.Initialise ();
			Mode = (byte)SensorMode;
			if (SensorMode != MSSensorMUXMode.TouchMode)
				NumberOfM = 3;
			else
				NumberOfM = 1;
			SetSensorMode ();
		}

		public MSSensorMUXBase (SensorPort Port, MSSensorMUXPort Address, UltraSonicMode SensorMode) : base (Port, (byte)Address, I2CMode.LowSpeed)
		{
			base.Initialise ();
			if (SensorMode == UltraSonicMode.Centimeter)
				Mode = 0;
			else if (SensorMode == UltraSonicMode.Inch)
				Mode = 1;
			else if (SensorMode == UltraSonicMode.Listen)
				Mode = 2;

			SetSensorMode ();
			NumberOfM = 2;
		}

		public MSSensorMUXBase (SensorPort Port, MSSensorMUXPort Address, ColorMode SensorMode) : base (Port, (byte)Address, I2CMode.LowSpeed)
		{
			base.Initialise ();
			if (SensorMode == ColorMode.Reflection)
				Mode = 0;
			else if (SensorMode == ColorMode.Ambient)
				Mode = 1;
			else if (SensorMode == ColorMode.Color)
				Mode = 2;

			NumberOfM = 2;
			SetSensorMode ();
		}

		public MSSensorMUXBase (SensorPort Port, MSSensorMUXPort Address, GyroMode SensorMode) : base (Port, (byte)Address, I2CMode.LowSpeed)
		{
			base.Initialise ();
			if (SensorMode == GyroMode.Angle)
				Mode = 0;
			else if (SensorMode == GyroMode.AngularVelocity)
				Mode = 1;

			NumberOfM = 1;
			SetSensorMode ();
		}

		public MSSensorMUXBase (SensorPort Port, MSSensorMUXPort Address, IRMode SensorMode) : base (Port, (byte)Address, I2CMode.LowSpeed)
		{
			base.Initialise ();
			if (SensorMode == IRMode.Proximity)
				Mode = 0;
			else if (SensorMode == IRMode.Seek)
				Mode = 1;
			else if (SensorMode == IRMode.Remote)
				Mode = 2;

			NumberOfM = 2;
			SetSensorMode ();
		}

		/// <summary>
		/// Reads one Byte from the Sensor.
		/// </summary>
		public byte Read()
		{
			return ReadOneByte ();
		}

		/// <summary>
		/// Reads one Byte from the Sensor.
		/// </summary>
		/// <returns>The read one byte.</returns>
		public byte ReadOneByte ()
		{
			byte[] Result;
			Result = base.ReadRegister ((byte)0x54, 1);
			return Result [0];
		}

		/// <summary>
		/// Reads two Bytes from the Sensor.
		/// </summary>
		/// <returns>The read two bytes.</returns>
		public byte[] ReadTwoBytes ()
		{
			byte[] Result;
			Result = base.ReadRegister ((byte)0x54, 2);
			return Result;
		}

		/// <summary>
		/// Reads four Bytes from the Sensor.
		/// </summary>
		/// <returns>The read four bytes.</returns>
		public byte[] ReadFourBytes ()
		{
			byte[] Result;
			Result = base.ReadRegister ((byte)0x54, 4);
			return Result;
		}

		/// <summary>
		/// Reads eight Bytes from the Sensor.
		/// </summary>
		/// <returns>The read eight bytes.</returns>
		public byte[] ReadEightBytes()
		{
			byte[] Result;
			Result = base.ReadRegister ((byte)0x54, 8);
			return Result;
		}

		private void SetSensorMode()
		{
			base.WriteRegister ((byte)0x52, Mode);
		}

		/// <summary>
		/// Reads the first byte from the sensor and returns the value.
		/// </summary>
		/// <returns>The value as string.</returns>
		public override string ReadAsString()
		{
			return (Convert.ToString (Read()));
		}

		public override void SelectPreviousMode()
		{
			if (Mode > 0)
				Mode--;
			else
				Mode = NumberOfM;
			SetSensorMode ();
		}

		public override int NumberOfModes()
		{
			return(NumberOfM);
		}

		public override string SelectedMode ()
		{
			return (Convert.ToString(Mode));
		}

		public override void SelectNextMode()
		{
			if (Mode < NumberOfM)
				Mode++;
			else
				Mode = 0;
		}

		public override string GetSensorName()
		{
			return ("EV3SMux");
		}
	}
}

