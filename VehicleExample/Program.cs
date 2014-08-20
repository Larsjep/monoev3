using System;
using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Movement;
namespace VehicleExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			
			Vehicle vehicle = new Vehicle (MotorPort.OutA, MotorPort.OutD);
			WaitHandle waitHandle;
			const int speed = 40;
			const int moveSteps = 2000;//You need to adjust this
			const int spinSteps = 500;//You need to adjust this
			vehicle.ReverseLeft = false;//You might need to adjust this
			vehicle.ReverseRight = false;//You might need to adjust this
			LcdConsole.WriteLine("Use Motor on A");
			LcdConsole.WriteLine("Use Motor on D");
			//Make a square
			for (int i = 0; i < 4; i++) {
				LcdConsole.WriteLine ("Spin left");
				waitHandle = vehicle.SpinLeft (speed, spinSteps, true);
				waitHandle.WaitOne();
				LcdConsole.WriteLine ("Move forward");
				waitHandle = vehicle.Forward (speed, moveSteps, true);
				waitHandle.WaitOne();
			}
			vehicle.Off ();
			
			Thread.Sleep(3000);
			LcdConsole.WriteLine("Make a soft turn to the left");
			waitHandle = vehicle.TurnLeftForward (speed, 50, moveSteps, false);
			waitHandle.WaitOne();
			LcdConsole.WriteLine("Make a soft turn to the left");
		}
	}
}

