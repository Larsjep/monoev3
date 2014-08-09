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
			
			Motor motorA = new Motor (MotorPort.OutA);
			Motor motorD = new Motor (MotorPort.OutD);
			
			/*motorA.ResetTacho();
			motorD.ResetTacho ();
			Console.WriteLine("Motor A: " + motorA.GetTachoCount().ToString());
			Console.WriteLine("Motor D: " + motorD.GetTachoCount().ToString());

			motorA.MoveTo(25,1000,true,false);
			motorD.MoveTo(25,1000,true,true);

			Console.WriteLine("Motor A: " + motorA.GetTachoCount().ToString());
			System.Threading.Thread.Sleep (1000);
			Console.WriteLine("Motor D: " + motorD.GetTachoCount().ToString());*/

		}
	}
}
