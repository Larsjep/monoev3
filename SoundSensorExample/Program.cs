using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;
using System.Threading;

namespace SoundSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent(false);
			var soundSensor = new SoundSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			buts.EscapePressed += () => { 
				terminateProgram.Set();
			};
			buts.UpPressed += () => { 
				LcdConsole.WriteLine("Sensor value:" + soundSensor.ReadAsString());
			};
			buts.EnterPressed += () => { 
				LcdConsole.WriteLine("Sensor raw value:" + soundSensor.ReadRaw());
			};
			buts.DownPressed += () => { 
				if(soundSensor.Mode == SoundMode.SoundDB){
					soundSensor.Mode = SoundMode.SoundDBA;
				}
				else{
					soundSensor.Mode = SoundMode.SoundDB;
				}
				LcdConsole.WriteLine("Sensor mode is now: " + soundSensor.Mode);
			};  
			terminateProgram.WaitOne();
		}
	}
}
