using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;
using MonoBrickFirmware.Graphics;
using MonoBrickFirmware.IO;
using System.Reflection;

namespace StartupApp
{
		
	
	class MainClass
	{
		static Bitmap monoLogo = Bitmap.FromResouce(Assembly.GetExecutingAssembly(), "monologo.bitmap");
		static Font font = Font.FromResource(Assembly.GetExecutingAssembly(), "info56_12.font");
		
		public static string GetIpAddress()
		{
			NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (var ni in interfaces)
			{
				foreach (var addr in ni.GetIPProperties().UnicastAddresses)
				{
					if (addr.Address.ToString() != "127.0.0.1")
						return addr.Address.ToString();					
				}
			}
			return "Unknown";
		}
		
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			Lcd lcd = new Lcd();			
			lcd.DrawBitmap(monoLogo, new Point((int)(Lcd.Width-monoLogo.Width)/2,0));
			
			string iptext = "IP: " + GetIpAddress();
			Point textPos = new Point((Lcd.Width-font.TextSize(iptext).x)/2, Lcd.Height-23);
			lcd.WriteText(font, textPos, iptext , true);
			lcd.Update();						
		}
	}
}
