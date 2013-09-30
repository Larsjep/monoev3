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
			motorA.On(20);
			System.Threading.Thread.Sleep(5000);
			motorA.Off();
			
		}
	}
}
