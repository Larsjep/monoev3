using System;
using MonoBrickFirmware.Tools;
using MonoBrickFirmware.Display;
namespace MonoBrickFirmware.Movement
{
	public class PositionPID: PIDAbstraction 
	{
		private Motor motor;
		private float target;
		private bool brake;
		private MovingAverage movingAverage = new MovingAverage(400);
		bool setAverage = true;
		public PositionPID (MotorPort port, Int32 position, bool brake, sbyte maxPower, float P, float I, float D, float sampleTime): 
		base(P,I,D,sampleTime, (float) maxPower, -((float) maxPower))   
		{
			this.motor = new Motor(port);
			target = position;
			this.brake = brake;
		}
		
		protected override void ApplyOutput (float output)
		{
			//output = -output;
			//Console.WriteLine("****** output ****" + output);
			if (output == 0.0f && brake) 
			{
				motor.Brake();	
			} 
			else 
			{
				motor.SetPower ((sbyte)output);		
			}
		}
		
		protected override float CalculateError ()
		{
			float currentPosition = ((float)motor.GetTachoCount());
			return (float)(target - currentPosition);	
		}
		
		protected override bool StopLoop ()
		{
			//Console.WriteLine("CurrentError: " + currentError);
			//Console.WriteLine("CurrentOutput: " + currentOutput);
			float average;
			if (setAverage) 
			{
				movingAverage.FillWindow(currentError);
				setAverage = false;
				LcdConsole.WriteLine("Current error:" + currentError.ToString());
				LcdConsole.WriteLine("Set average:" + setAverage);
				//Console.WriteLine("Set average");
			} 
			else 
			{
				movingAverage.Update(currentError);
			}
			average = movingAverage.GetAverage();	
			//Console.WriteLine("Average: " + average);
			if (average <= 5.0f && average >= -5.0f) {
				//Console.WriteLine("Within limits");
				//Console.WriteLine("Average " + average);
				if(brake)
					motor.Brake();
				else
					motor.SetPower(0);
				setAverage = true;
				return true;
			}
			return false;
			/*bool errorOk = currentError == 0;
			//Console.WriteLine("Error ok:" + errorOk);
			bool outputOk = currentOutput == 0;
			//Console.WriteLine("output ok:" + outputOk);
			return errorOk && outputOk;*/
		}
		
	}
}

