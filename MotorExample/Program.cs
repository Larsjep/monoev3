using System;
using MonoBrickFirmware.IO;
namespace MotorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Running single motor test");
			Motor motor = new Motor(MotorPort.OutA);
			Console.WriteLine ("Reset motor tacho");
			motor.ResetTacho();
			Console.WriteLine ("Running forward with 20");
			motor.On(20);
			System.Threading.Thread.Sleep(2500);
			Console.WriteLine ("Motor speed: " + motor.GetSpeed());
			System.Threading.Thread.Sleep(2500);
			Console.WriteLine ("Running backwards with 70");
			motor.On(-70);
			System.Threading.Thread.Sleep(3000);
			Console.WriteLine ("Reverse direction");
			motor.Reverse = true;
			System.Threading.Thread.Sleep(3000);
			Console.WriteLine ("Brake");
			motor.Reverse = false;
			motor.Brake();
			System.Threading.Thread.Sleep(3000);			
			Console.WriteLine ("Off");
			motor.Off();
			System.Threading.Thread.Sleep(3000);			
			Console.WriteLine ("Move to zero");
			Console.WriteLine (motor.GetTachoCount());
			motor.MoveTo(10, 0, true);
			Console.WriteLine ("Done executing single motor test");
			
			
			
		}
	}
}
