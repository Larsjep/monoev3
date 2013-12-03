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
using System.Linq;

namespace StartupApp
{
		
	
	class MainClass
	{
		static Bitmap monoLogo = Bitmap.FromResouce(Assembly.GetExecutingAssembly(), "monologo.bitmap");
		static Font font = Font.MediumFont;
		static Action MenuAction = null;
		
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
		
		static bool Information(Lcd lcd, Buttons btns)
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
			return false;
		}
		
		static bool StartApp(string filename)
		{
			MenuAction = () => RunAndWaitForProgram("/usr/local/bin/mono", filename);
			return true;
		}
		
		static string GetFileNameWithoutExt(string fullname)
		{
			string filename = new FileInfo(fullname).Name;
			return filename.Substring(0, filename.Length-4);
		}
		
		static bool RunPrograms(Lcd lcd, Buttons btns)
		{
			IEnumerable<MenuItem> items = Directory.EnumerateFiles("/home/root/apps/", "*.exe")
				.Select( (filename) => new MenuItem(lcd, GetFileNameWithoutExt(filename),  () => StartApp(filename)));
			Menu m = new Menu(font, lcd, "Run program:", items);
			m.ShowMenu(btns);
			return true;
		}
		
		static void RunAndWaitForProgram(string filename, string arguments = "")
		{
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.EnableRaisingEvents=false; 
			Console.WriteLine("Starting process: {0} with arguments: {1}", filename, arguments);
			proc.StartInfo.FileName = filename;
			proc.StartInfo.Arguments = arguments;
			proc.Start();
			proc.WaitForExit();
		}
		
		static bool Shutdown(Lcd lcd, Buttons btns)
		{
			lcd.Clear();
			lcd.WriteText(font, new Point(0,0), "Shutting down...", true);
			lcd.Update();
			
			using (UnixDevice dev = new UnixDevice("/dev/lms_power"))
			{
				dev.IoCtl(0, new byte[0]);
			}
			btns.LedPattern(2);
			MenuAction = () => RunAndWaitForProgram("/lejos/bin/exit", "");			
			return true;
		}
		
		static void ShowMainMenu(Lcd lcd, Buttons btns)
		{
			
			List<MenuItem> items = new List<MenuItem>();
			items.Add (new MenuItem(lcd, "Information", () => Information(lcd, btns)));
			items.Add (new MenuItem(lcd, "Run programs", () => RunPrograms(lcd, btns)));
			items.Add (new MenuItem(lcd, "Shutdown", () => Shutdown(lcd,btns)));
			items.Add (new MenuItem(lcd, "Settings", () => ShowSettings(lcd,btns)));
			Menu m = new Menu(font, lcd, "Main menu", items);
			m.ShowMenu(btns);
		}
		
		static bool ShowSettings(Lcd lcd, Buttons btns)
		{
			List<IMenuItem> items = new List<IMenuItem>();
			items.Add(new MenuItemWithOptions(lcd,"Remote debug",new string[]{"On", "Off"},0));
			items.Add(new MenuItemWithOptions(lcd,"EnableBT",new string[]{"Yes", "No"},0));
			Menu m = new Menu(font, lcd, "Settings", items);
			m.ShowMenu(btns);
			return false;
		}
		
		
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			using (Lcd lcd = new Lcd())
				using (Buttons btns = new Buttons())
				{					
					lcd.DrawBitmap(monoLogo, new Point((int)(Lcd.Width-monoLogo.Width)/2,0));					
					string iptext = "IP: " + GetIpAddress();
					Point textPos = new Point((Lcd.Width-font.TextSize(iptext).x)/2, Lcd.Height-23);
					lcd.WriteText(font, textPos, iptext , true);
					lcd.Update();						
					btns.GetKeypress();
				}
			for (;;)
			{
				using (Lcd lcd = new Lcd())
					using (Buttons btns = new Buttons())
					{
						ShowMainMenu(lcd, btns);					
					}			
				if (MenuAction != null)
				{
					Console.WriteLine("Starting application");
					MenuAction();					
					Console.WriteLine ("Done running application");
					MenuAction = null;
				}
			}
		}
	}		
}
