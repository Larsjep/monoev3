using System;
using MonoBrickFirmware.IO;
namespace TouchSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			bool run = true;
			//var sensor1 = new AnalogSensor(SensorPort.In1);
			var sensor1 = new Uart(SensorPort.In1);
			sensor1.SetOperatingMode(SensorMode.Mode2);
			//sensor1.InitialiseSensor(SensorMode.Mode0);
			//sensor1.Reset();
			Buttons buts = new Buttons ();
			buts.EnterPressed += () => { 
				run  = false;
			};
			buts.UpPressed += () => { 
				/*Console.WriteLine("Pin1: " + sensor1.ReadPin1());
				Console.WriteLine("Pin5: " + sensor1.ReadPin5());
				Console.WriteLine("Pin6: " + sensor1.ReadPin6());*/
				sensor1.SetMode(SensorMode.Mode2);
				Console.WriteLine("Raw float:" + sensor1.ReadBytes(4));
			};
			buts.DownPressed += () => { 
				//Console.WriteLine("Connection type: " + sensor1.GetConnectionType());
				//Console.WriteLine("Sensor type: " + sensor1.GetAnalogSensorType());
			};  
			while (run) {
				System.Threading.Thread.Sleep(50);
			}
		}
	}
}
