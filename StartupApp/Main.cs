using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;
using MonoBrickFirmware.Graphics;
using MonoBrickFirmware.IO;
using System.Reflection;
using System.Collections.Generic;
using MonoBrickFirmware.Native;

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
		
		static void Information(Lcd lcd, Buttons btns)
		{
			string monoVersion = "Unknown";
			Type type = Type.GetType("Mono.Runtime");
			if (type != null)
			{                                          
	    		MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static); 
	    		if (displayName != null)                   
	        		monoVersion = (string)displayName.Invoke(null, null); 
			}	
			string monoCLR = System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion;
			
			Point offset = new Point(0, (int)font.maxHeight);
			Point startPos = new Point(0,0);
			lcd.Clear();
			lcd.WriteText(font, startPos+offset*0, "MonoBrickFirmware:", true);
			lcd.WriteText(font, startPos+offset*1, "0.1.0.0", true);
			lcd.WriteText(font, startPos+offset*2, "Mono version:", true);
			lcd.WriteText(font, startPos+offset*3, monoVersion, true);
			lcd.WriteText(font, startPos+offset*4, "Mono CLR:" + monoCLR, true);			
			lcd.Update();
			btns.GetKeypress();
		}
		
		static void RunPrograms()
		{
		}
		
		static void Shutdown(Lcd lcd, Buttons btns)
		{
			lcd.Clear();
			lcd.WriteText(font, new Point(0,0), "Shutting down...", true);
			lcd.Update();
			
			UnixDevice dev = new UnixDevice("/dev/lms_power");
			dev.IoCtl(0, new byte[0]);
			btns.LedPattern(2);
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents=false; 
			proc.StartInfo.FileName = "poweroff";
			proc.StartInfo.Arguments = "-f";
			proc.Start();
			proc.WaitForExit();
			for (;;); // The system should now shutdown.
		}
		
		static void ShowMainMenu(Lcd lcd, Buttons btns)
		{
			
			List<MenuItem> items = new List<MenuItem>();
			items.Add (new MenuItem() { text = "Information", action = () => Information(lcd, btns) });
			items.Add (new MenuItem() { text = "Run programs", action = RunPrograms });
			items.Add (new MenuItem() { text = "Shutdown", action = () => Shutdown(lcd,btns) });			
			
			Menu m = new Menu(font, lcd, "Main menu", items);
			m.ShowMenu(btns);
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
			Buttons btns = new Buttons();
			btns.GetKeypress();
			for (;;)
				ShowMainMenu(lcd, btns);			
		}
	}		
}
