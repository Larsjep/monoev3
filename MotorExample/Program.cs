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
			MotorSync motorSync = new MotorSync(MotorPort.OutA, MotorPort.OutD);
			LcdConsole.WriteLine("Use Motor on A");
			LcdConsole.WriteLine("Use Motor on D");
			LcdConsole.WriteLine("Press Ent. to start");
			Buttons.Instance.GetKeypress();
			motor.ResetTacho ();
			LcdConsole.WriteLine ("Running forward with 20");
			motor.On(20);
			System.Threading.Thread.Sleep(2500);
			LcdConsole.WriteLine ("Printing motor speed: " + motor.GetSpeed());
			System.Threading.Thread.Sleep(2500);
			LcdConsole.WriteLine ("Running backwards with 70");
			motor.On(-70);
			System.Threading.Thread.Sleep(3000);
			LcdConsole.WriteLine ("Reverse direction");
			motor.Reverse = true;
			System.Threading.Thread.Sleep(3000);
			LcdConsole.WriteLine ("Brake");
			motor.Reverse = false;
			motor.Brake();
			System.Threading.Thread.Sleep(3000);			
			LcdConsole.WriteLine ("Off");
			motor.Off();
			System.Threading.Thread.Sleep(3000);			
			
			LcdConsole.WriteLine ("Move to zero");
			motor.MoveTo(40, 0, true);
			LcdConsole.WriteLine("Motor at position: " + motor.GetTachoCount());
			System.Threading.Thread.Sleep(2000);
			
			LcdConsole.WriteLine ("Creating a step profile");
			motor.SpeedProfileStep(40,100, 1500, 100, true);
			motor.Off();
			LcdConsole.WriteLine("Motor at position: " + motor.GetTachoCount());
			System.Threading.Thread.Sleep(2000);
			
			LcdConsole.WriteLine ("Motor " + motorSync.BitField + " synchronised forward for 2500 steps");
			motorSync.On(50, 0, 2500, true);
			motorSync.Off();
			LcdConsole.WriteLine ("Motor " + motorSync.BitField + " synchronised with second motor moving at half speed");
			motorSync.On(-20,50,2500, false); //coast when done
			
			
			
			LcdConsole.WriteLine ("Done executing motor test");
		}
	}
}
