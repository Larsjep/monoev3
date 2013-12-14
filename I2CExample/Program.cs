using System;
using System.Threading;
using System.Collections.Generic;
using System.Resources;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;


namespace I2CExample
{
	class MainClass
	{
		
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent(false);
			ButtonEvents buts = new ButtonEvents ();
			I2CSensor sensor = new I2CSensor(SensorPort.In1, 0x02, I2CMode.LowSpeed9V);
			buts.EscapePressed += () => { 
				terminateProgram.Set();
			};
			buts.UpPressed += () => { 
				LcdConsole.WriteLine("Write 0x02 to register 0x41");
				sensor.WriteRegister(0x41,0x02);
			};
			buts.DownPressed += () => { 
				LcdConsole.WriteLine("Write zero to register 0x41");
				sensor.WriteRegister(0x41,0x00);	 
			};
			buts.RightPressed += () => { 
				LcdConsole.WriteLine("Read type");
				byte[] i2cData = sensor.ReadRegister(0x00,I2CAbstraction.BufferSize);
				for(int i = 0; i < i2cData.Length;i++) {
						LcdConsole.WriteLine ("Data[{0}]: {1:X} Char: " + Convert.ToChar(i2cData[i]),i,i2cData[i]);
				}
			};
			terminateProgram.WaitOne();  
		}
		
		
	}
}
