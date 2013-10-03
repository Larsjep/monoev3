using System;
using MonoBrickFirmware.IO;
namespace MotorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Running motor test");
			Motor motorA = new Motor(MotorPort.OutA);
			Console.WriteLine ("Reset motor tacho");
			motorA.ResetTacho();
			Console.WriteLine ("Running forward with 20");
			motorA.On(20);
			System.Threading.Thread.Sleep(5000);
			Console.WriteLine ("Running backwards with 70");
			motorA.On(-70);
			System.Threading.Thread.Sleep(3000);
			Console.WriteLine ("Reverse direction");
			motorA.Reverse = true;
			System.Threading.Thread.Sleep(3000);
			Console.WriteLine ("Brake");
			motorA.Reverse = false;
			motorA.Brake();
			System.Threading.Thread.Sleep(3000);			
			Console.WriteLine ("Off");
			motorA.Off();
			System.Threading.Thread.Sleep(3000);			
			Console.WriteLine ("Move to zero");
			Console.WriteLine (motorA.GetTachoCount());
			motorA.MoveTo(10, 0, true);
			Console.WriteLine ("done executing");
			
			
		}
	}
}
