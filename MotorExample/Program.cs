using System;
using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MotorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			
			Motor motor = new Motor (MotorPort.OutA);
			motor.ResetTacho();
			Console.WriteLine(motor.GetTachoCount().ToString());
			motor.MoveTo(25,1000,true,true);
			System.Threading.Thread.Sleep(300);
			Console.WriteLine(motor.GetTachoCount().ToString());
			motor.ResetTacho();
			Console.WriteLine(motor.GetTachoCount().ToString());
			motor.PowerProfileStep (25, 200, 600, 200, true, true);
			Console.WriteLine(motor.GetTachoCount().ToString());
			System.Threading.Thread.Sleep (1000);
			Console.WriteLine(motor.GetTachoCount().ToString());
		}
	}
}
