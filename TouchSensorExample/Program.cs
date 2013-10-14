using System;
using MonoBrickFirmware.IO;
namespace TouchSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var uart = new Uart ();
			while (true) {
				System.Threading.Thread.Sleep(1000);
				uart.ReadMemory();
			}
			
			
			
			
			
		}
	}
}
