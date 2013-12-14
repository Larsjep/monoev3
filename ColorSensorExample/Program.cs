using System;
using MonoBrickFirmware;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Buttons;
using MonoBrickFirmware.Sensors;

using System.Threading;
namespace ColorSensorExample
{
class MainClass
{
	public static void Main (string[] args)
	{
		EventWaitHandle stopped = new ManualResetEvent(false);
		ColorMode[] modes = {ColorMode.Color, ColorMode.Reflection, ColorMode.Ambient, ColorMode.Blue};
		int modeIdx = 0;
		var sensor = new ColorSensor(SensorPort.In1);
		ButtonEvents buts = new ButtonEvents ();
		
		buts.EscapePressed += () => { 
			stopped.Set();
		};
		buts.UpPressed += () => { 
			LcdConsole.WriteLine("Sensor value: " + sensor.ReadAsString());
		};
		buts.DownPressed += () => { 
			LcdConsole.WriteLine("Raw sensor value: " + sensor.ReadRaw());
		};
		buts.EnterPressed += () => { 
			modeIdx = (modeIdx+1)%modes.Length;
			sensor.Mode = modes[modeIdx];
			LcdConsole.WriteLine("Sensor mode is set to: " + modes[modeIdx]);
		};  
		stopped.WaitOne();
	}
}
}
