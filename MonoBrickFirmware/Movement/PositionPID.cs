using System;
using MonoBrickFirmware.Tools;
using MonoBrickFirmware.Display;

namespace MonoBrickFirmware.Movement
{
	internal class PositionPID: PIDAbstraction 
	{
		private Output motor;
		private MotorPort port;
		private float target;
		private bool brake;
		private MovingAverage movingAverage = new MovingAverage(800);
		bool setAverage = true;
		public PositionPID (Output output, Int32 position, bool brake, sbyte maxPower, float P, float I, float D, float sampleTime): 
		base(P,I,D,sampleTime, (float) maxPower, -((float) maxPower))   
		{
			this.motor = output;
			switch (output.BitField) 
			{
				case OutputBitfield.OutA:
					port = MotorPort.OutA;
				break;
				case OutputBitfield.OutB:
					port = MotorPort.OutB;
				break;
				case OutputBitfield.OutC:
					port = MotorPort.OutC;
				break;
				case OutputBitfield.OutD:
					port = MotorPort.OutD;
				break;
			}
			target = position;
			this.brake = brake;
		}

		protected override void ApplyOutput (float output)
		{
			if (output == 0.0f && brake) 
			{
				motor.Stop (true);
			} 
			else 
			{
				motor.SetPower((sbyte)output);
				motor.Start ();
			}
		}
		
		protected override float CalculateError ()
		{
			float currentPosition = ((float)motor.GetCount(port));
			return (float)(target - currentPosition);	
		}
		
		protected override bool StopLoop ()
		{
			float average;
			if (setAverage) 
			{
				movingAverage.FillWindow(currentError);
				setAverage = false;
			} 
			else 
			{
				movingAverage.Update(currentError);
			}
			average = movingAverage.GetAverage();	
			if (average <= 1.0f && average >= -1.0f) {
				Console.WriteLine("Within limits");
				Console.WriteLine("Average " + average);
				if (brake) 
				{
					motor.Stop (true);
				}
				else 
				{
					motor.SetPower (0);
					motor.Stop (false);
				}
				setAverage = true;
				return true;
			}
			bool errorOk = currentError == 0;
			bool outputOk = currentOutput == 0;
			return errorOk && outputOk;
		}
	}
}

