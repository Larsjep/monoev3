using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;
using System.Threading;
namespace IRSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			IRChannel[] channels = {IRChannel.One, IRChannel.Two, IRChannel.Three, IRChannel.Four};
			
			int channelIdx = 0;
			ManualResetEvent terminateProgram = new ManualResetEvent(false);
			var sensor = new EV3IRSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			LcdConsole.WriteLine("Use IR on port1");
			LcdConsole.WriteLine("Up distance");
			LcdConsole.WriteLine("Down beacon location");
			LcdConsole.WriteLine("Enter read command");
			LcdConsole.WriteLine("Left change channel");
			LcdConsole.WriteLine("Right read as string");
			LcdConsole.WriteLine("Esc. terminate");
			buts.EscapePressed += () => { 
				terminateProgram.Set();
			};
			buts.UpPressed += () => { 
				LcdConsole.WriteLine("Distance " + sensor.ReadDistance() +  " cm");
			};
			buts.EnterPressed += () => { 
				LcdConsole.WriteLine("Remote command " + sensor.ReadRemoteCommand() + " on channel " + sensor.Channel);									
			};
			buts.DownPressed += () => { 
				BeaconLocation location  = sensor.ReadBeaconLocation();
				LcdConsole.WriteLine("Beacon location: " + location.Location + " Beacon distance: " + location.Distance + " cm"); 
				
			};
			buts.LeftPressed += () => { 
				channelIdx = (channelIdx+1)%channels.Length;
				sensor.Channel = channels[channelIdx];
				LcdConsole.WriteLine("Channel is set to: " + channels[channelIdx]);
			};
			buts.RightPressed += () => { 
				LcdConsole.WriteLine(sensor.ReadAsString());	
			};
			terminateProgram.WaitOne(); 
			
		}
	}
}
