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
			motor.MoveTo(25,1000,true,true);
			System.Threading.Thread.Sleep(3000);
			Console.WriteLine(motor.GetTachoCount().ToString());
			
			/*motor.MoveTo(75,0,false,true);
			LcdConsole.WriteLine(motor.GetTachoCount().ToString());
			System.Threading.Thread.Sleep(3000);
			LcdConsole.WriteLine ("Done executing motor test");*/
		}
	}
}
