using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoBrickWebServer.Models
{
	public interface IMotorModel
	{
		string Port { get; }
		int Speed { get;}
		int Position {get;}
		void SetSpeed (sbyte speed);
		void SpeedProfile (sbyte speed, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake);
		void PowerProfile (sbyte power, UInt32 rampUpSteps, UInt32 constantSpeedSteps, UInt32 rampDownSteps, bool brake);
		void SetPower(sbyte power);
		void Off();
		void Break();
		void ResetTacho();
		void Update();
	}
}