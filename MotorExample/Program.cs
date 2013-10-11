using System;
using MonoBrickFirmware.IO;
namespace MotorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Motor motor = new Motor (MotorPort.OutA);
			MotorSync motorSync = new MotorSync(MotorPort.OutA, MotorPort.OutD);
			
			Console.WriteLine ("Running single motor test");
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
			Console.WriteLine("Motor at position: " + motor.GetTachoCount());
			System.Threading.Thread.Sleep(1000);
			
			Console.WriteLine ("Creating a step profile");
			motor.SpeedProfileStep(40,300, 500, 300, true);
			motor.Off();
			Console.WriteLine("Motor at position: " + motor.GetTachoCount());
			System.Threading.Thread.Sleep(1000);
			
			Console.WriteLine ("Motor " + motorSync.BitField + " synchronised forward for 2500 steps");
			motorSync.On(50, 0, 2500, true);
			motorSync.Off();
			Console.WriteLine ("Mortor " + motorSync.BitField + " synchronised backwards for 2500 steps");
			Console.WriteLine("Second motor moving at half speed");
			motorSync.On(-20,50,2500, false); //coast when done
			
			
			
			Console.WriteLine ("Done executing motor test");
		}
	}
}
