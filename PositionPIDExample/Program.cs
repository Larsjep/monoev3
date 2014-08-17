using System;
using System.Threading;
using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace PositionPIDExample
{
	class MainClass
	{
		private const float P = 4.8f;
		private const float I = 800.1f;
		private const float D = 8.5f;


		public static void Main (string[] args)
		{
			
			Motor motor = new Motor(MotorPort.OutA);
			motor.ResetTacho();
			PositionPID PID = new PositionPID(motor, 4000, true, 50, P, I, D, 5000);
			var waitHandle = PID.Run();
			Console.WriteLine("Moving motor A to position 4000");
			Console.WriteLine("Waiting for controller to finish");
			//Wait for controller to finish - you can do other stuff here
			waitHandle.WaitOne();
			Console.WriteLine("Done");
			Console.WriteLine("Motor position: " + motor.GetTachoCount());
		}
	}
}
