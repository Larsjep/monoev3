using System;
using System.Threading;
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
			
			motorA.Off();
			motorD.Off();

			motorA.ResetTacho();
			motorD.ResetTacho();

			LcdConsole.WriteLine("Set speed to 50");
			motorA.SetSpeed(50);
			Thread.Sleep(1000);
			LcdConsole.WriteLine("Speed: " + motorA.GetSpeed());
			Thread.Sleep(2000);
			LcdConsole.WriteLine("Break");
			motorA.Brake();

			Thread.Sleep(3000);
			motorA.ResetTacho();
			LcdConsole.WriteLine("Moving motor A to 2000 ");
			var motorWaitHandle =  motorA.SpeedProfile(50, 200, 1600, 200,true);
			LcdConsole.WriteLine("Waiting for motor A to stop");
			//you could do something else here
			motorWaitHandle.WaitOne();
			LcdConsole.WriteLine("Done moving motor");
			LcdConsole.WriteLine("Position A: " + motorA.GetTachoCount());


			Thread.Sleep(3000);
			motorA.ResetTacho();
			motorD.ResetTacho();
			LcdConsole.WriteLine("Moving motors A to 2000");
			LcdConsole.WriteLine("Moving motor B to 1000");
			WaitHandle[] handles = new WaitHandle[2];
			handles[0] =  motorA.SpeedProfile(50, 200, 1600, 200,true);
			handles[1] = motorD.SpeedProfile(50,200,600,200,true);
			LcdConsole.WriteLine("Waiting for both motors to stop");
			//you could do something else here
			WaitHandle.WaitAll(handles);
			LcdConsole.WriteLine("Done moving both motors");
			LcdConsole.WriteLine("Position A: " + motorA.GetTachoCount());
			LcdConsole.WriteLine("Position D: " + motorD.GetTachoCount());
			motorA.Off();
			motorD.Off();
			
		}
	}
}
