using System.Collections;
using MonoBrickFirmware.Movement;

using System;

namespace MonoBrickWebServer.Models
{
	public class DummyMotorModel : IMotorModel
	{
		public DummyMotorModel(MonoBrickFirmware.Movement.MotorPort port)
		{
			Port = port.ToString();
		}

		public string Port { get; private set; }
		public int Speed { get; private set; }
		public int Position { get; private set; }

		public void SetSpeed (sbyte speed)
		{
			Position++;
			Speed = speed;
		}

		public void SpeedProfile (sbyte speed, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake)
		{
			if (speed < 0) 
			{
				Position = (int)rampUpSteps - (int)rampDownSteps - (int)constantSpeedSteps;
			} 
			else 
			{
				Position = (int)rampUpSteps + (int)rampDownSteps + (int)constantSpeedSteps;
			}
			Speed = speed;
		}

		public void PowerProfile (sbyte power, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake)
		{
			if (power < 0) 
			{
				Position = (int)rampUpSteps - (int)rampDownSteps - (int)constantSpeedSteps;
			} 
			else 
			{
				Position = (int)rampUpSteps + (int)rampDownSteps + (int)constantSpeedSteps;
			}
			Speed = power;
		}

		public void SetPower(sbyte power)
		{
			Speed = power;
		}

		public void Off()
		{
			Speed = 0;
		}

		public void Break()
		{
			Speed = 0;
			Position = 0;
		}

		public void Move(int speed, int steps)
		{
			Speed = speed;
			Position = Position + steps;
		}

		public void ResetTacho()
		{
			Speed = 0;
			Position = 0;
		}

		public void Update()
		{
		}
	}
}

