using System;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Sensors;
using System.Threading;
namespace TouchSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent(false);
			var touchSensor = new TouchSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			buts.EnterPressed += () => { 
				terminateProgram.Set();
			};
			buts.UpPressed += () => { 
				LcdConsole.WriteLine("Sensor value:" + touchSensor.ReadAsString());
			};
			buts.DownPressed += () => { 
				LcdConsole.WriteLine("Raw sensor value: " + touchSensor.ReadRaw());
			};  
			terminateProgram.WaitOne();
		}
	}
}
