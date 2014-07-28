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
			
			
			Console.WriteLine("This is the start of my program");
			Motor motor = new Motor (MotorPort.OutA);
			Console.WriteLine("Constructor has been called");
			//motor.ResetTacho();
			//motor.MoveTo(25,1000,true,true);
			motor.SetSpeed(40);
			System.Threading.Thread.Sleep(300);
			Console.WriteLine(motor.GetTachoCount().ToString());
			
			/*motor.MoveTo(75,0,false,true);
			LcdConsole.WriteLine(motor.GetTachoCount().ToString());
			System.Threading.Thread.Sleep(3000);
			LcdConsole.WriteLine ("Done executing motor test");*/
		}
	}
}
