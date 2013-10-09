using System;
using MonoBrickFirmware.IO;
namespace MotorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Running single motor test");
			Motor motor = new Motor (MotorPort.OutA);
			Console.WriteLine ("Reset motor tacho");
			motor.Brake ();
			System.Threading.Thread.Sleep (2500);
			motor.ResetTacho ();
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
			motor.MoveTo(40, 0, true);
			do{
				System.Threading.Thread.Sleep(200);
				Console.WriteLine("Tacho count: " + motor.GetTachoCount());
				Console.WriteLine("Speed:" + motor.GetSpeed());
			}while(motor.IsRunning());
			Console.WriteLine("Motor at position: " + motor.GetTachoCount());
			System.Threading.Thread.Sleep(2500);
			
			
			
			/*motor.Brake();
			Console.WriteLine ("Start position:" + motor.GetTachoCount ());
			System.Threading.Thread.Sleep(2500);
			motor.SpeedProfileStep (10, 255, 255, 255, false);
			System.Threading.Thread.Sleep (500);
			while (motor.IsRunning()) {
				System.Threading.Thread.Sleep (100);
				Console.WriteLine("Tacho count:" + motor.GetTachoCount());
				Console.WriteLine(motor.IsRunning());
				Console.WriteLine("Speed:" + motor.GetSpeed());

			}
			Console.WriteLine ("End position:" + motor.GetTachoCount ());*/
			
			Console.WriteLine ("Done executing single motor test");
		}
	}
}
