using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace I2CExample
{
	class MainClass
	{
		
		public static void Main (string[] args)
		{
			Lcd lcd = new Lcd();
			Font f = Font.FromResource(System.Reflection.Assembly.GetExecutingAssembly(), "font.info56_12");
			Point offset = new Point(0,25);
			Point p = new Point(10, Lcd.Height-75);
			Point boxSize = new Point(100, 24);
			Rect box = new Rect(p, p+boxSize);
			ManualResetEvent terminateProgram = new ManualResetEvent(false);
			ButtonEvents buts = new ButtonEvents ();
			I2CSensor sensor = new I2CSensor(SensorPort.In1, 0x02, I2CMode.LowSpeed9V);
			buts.EscapePressed += () => { 
				terminateProgram.Set();
			};
			buts.UpPressed += () => { 
				Console.WriteLine("Sonar sensor On");
				sensor.WriteRegister(0x41,0x02);
			};
			buts.DownPressed += () => { 
				Console.WriteLine("Sonar sensor Off");
				sensor.WriteRegister(0x41,0x00);//off	 
			};
			buts.LeftPressed += () => { 
				
			};
			buts.RightPressed += () => { 
				Console.WriteLine("Read type");
				byte[] i2cData = sensor.ReadRegister(0x42,8);
				for(int i = 0; i < i2cData.Length;i++) {
						Console.WriteLine ("Data[{0}]: {1:X} Char: " + Convert.ToChar(i2cData[i]),i,i2cData[i]);
				}
			};
			
			
			terminateProgram.WaitOne();  
			
		    		
		}
		
		
	}
}
