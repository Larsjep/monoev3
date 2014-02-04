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
			var touchSensor = new EV3TouchSensor(SensorPort.In1);
			//svar touchSensor = new NXTTouchSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			LcdConsole.WriteLine("Use touch on port1");
			LcdConsole.WriteLine("Up read");
			LcdConsole.WriteLine("Down read raw");
			LcdConsole.WriteLine("Esc. terminate");
			buts.EscapePressed += () => { 
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
