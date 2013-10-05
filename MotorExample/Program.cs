using System;
using MonoBrickFirmware.IO;
namespace MotorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			/*Console.WriteLine ("Running motor test");
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
			Console.WriteLine ("done executing");*/
			
			Console.WriteLine ("Running motor test");
			Motor motorA = new Motor (MotorPort.OutA);
			Motor motorB = new Motor (MotorPort.OutB);
			Motor motorC = new Motor (MotorPort.OutC);
			Motor motorD = new Motor(MotorPort.OutD);
			motorA.ResetTacho();
			motorB.ResetTacho();
			motorC.ResetTacho();
			motorD.ResetTacho();
			motorA.Off();
			motorB.Off();
			motorC.Off();
			motorD.Off();
			motorA.GetTachoCount();
			Motor motorToMove = motorA;
			for(int i = 0; i < 5; i++){
				System.Threading.Thread.Sleep(5000);
				Console.WriteLine("");
				Console.WriteLine("***** new moving timer ******");
				motorToMove.On(-20);
				Console.WriteLine("Count:" + motorToMove.GetTachoCount());
				Console.WriteLine("Speed:" + motorToMove.GetSpeed());
						
			}
			motorToMove.Brake();
			motorToMove.Off();
			Console.WriteLine ("Done moving");
			for(int i = 0; i < 5; i++){
				System.Threading.Thread.Sleep(5000);
				Console.WriteLine("");
				Console.WriteLine("***** new stop timer ******");
				Console.WriteLine("Count:" + motorToMove.GetTachoCount());
			}
			Console.WriteLine ("Done");
			Console.WriteLine ("Now I am done");
			
		}
	}
}
