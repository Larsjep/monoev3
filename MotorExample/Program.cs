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
			WaitHandle motorWaitHandle;
			motorA.Off();
			motorD.Off();

			//Power control
			LcdConsole.WriteLine("Set power to 50");
			motorA.SetPower(50);
			Thread.Sleep(3000);
			LcdConsole.WriteLine("Break");
			motorA.Brake();

			//Speed control
			LcdConsole.WriteLine("Set speed to 50");
			motorA.SetSpeed(50);
			Thread.Sleep(1000);
			LcdConsole.WriteLine("Speed: " + motorA.GetSpeed());
			Thread.Sleep(2000);
			LcdConsole.WriteLine("Break");
			motorA.Brake();

			//Position control of single motor
			Thread.Sleep(3000);
			motorA.ResetTacho();
			LcdConsole.WriteLine("Moving motor A to 2000 ");
			motorWaitHandle =  motorA.SpeedProfile(50, 200, 1600, 200,true);
			//you could do something else here
			LcdConsole.WriteLine("Waiting for motor A to stop");
			motorWaitHandle.WaitOne();
			LcdConsole.WriteLine("Done moving motor");
			LcdConsole.WriteLine("Position A: " + motorA.GetTachoCount());

			//Individual position control of both motors
			Thread.Sleep(3000);
			motorA.ResetTacho();
			motorD.ResetTacho();
			LcdConsole.WriteLine("Moving motors A to 2000");
			LcdConsole.WriteLine("Moving motor B to 1000");
			WaitHandle[] handles = new WaitHandle[2];
			handles[0] =  motorA.SpeedProfile(50, 200, 1600, 200,true);
			handles[1] = motorD.SpeedProfile(50,200,600,200,true);
			//you could do something else here
			LcdConsole.WriteLine("Waiting for both motors to stop");
			WaitHandle.WaitAll(handles);
			LcdConsole.WriteLine("Done moving both motors");
			LcdConsole.WriteLine("Position A: " + motorA.GetTachoCount());
			LcdConsole.WriteLine("Position D: " + motorD.GetTachoCount());
			motorA.Off();
			motorD.Off();

			//Motor synchronisation position control 
			Thread.Sleep(3000);
			motorA.ResetTacho();
			motorD.ResetTacho();
			MotorSync sync = new MotorSync(MotorPort.OutA, MotorPort.OutD);
			LcdConsole.WriteLine("Sync motors to move 3000 steps forward");
			motorWaitHandle = sync.StepSync(40,0, 3000, true);
			//you could do something else here
			LcdConsole.WriteLine("Waiting for sync to stop");
			motorWaitHandle.WaitOne();
			LcdConsole.WriteLine("Done sync moving both motors");
			LcdConsole.WriteLine("Position A: " + motorA.GetTachoCount());
			LcdConsole.WriteLine("Position D: " + motorD.GetTachoCount());
			sync.Off();

			
		}
	}
}
