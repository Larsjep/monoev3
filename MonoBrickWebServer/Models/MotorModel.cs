using System;
using System.Collections;
using MonoBrickFirmware.Movement;

namespace MonoBrickWebServer.Models
{
	public class MotorModel : IMotorModel
	{
		private Motor motor;
	    
		public MotorModel (MonoBrickFirmware.Movement.MotorPort port)
		{
			Port = port.ToString();
			motor = new Motor(port);
			Update();
		}

		public string Port { get; private set; }
		public int Speed { get; private set; }
		public int Position { get; private set; }

		public void SetSpeed (sbyte speed)
		{
			motor.SetSpeed (speed);
		}

		public void SpeedProfile (sbyte speed, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake)
		{
			motor.SpeedProfile (speed, rampUpSteps, constantSpeedSteps, rampDownSteps, brake);
		}

		public void PowerProfile (sbyte power, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake)
		{
			motor.PowerProfile (power, rampUpSteps, constantSpeedSteps, rampDownSteps, brake);
		}

		public void SetPower(sbyte power)
		{
			motor.SetPower (power);
		}

		public void Off()
		{
			motor.Off();
		}

		public void Break()
		{
			motor.Brake();
		}
		
		public void ResetTacho ()
		{
			motor.ResetTacho();
		}

		public void Update()
		{
			Speed = motor.GetSpeed();
			Position = motor.GetTachoCount();
		}
	}
}

