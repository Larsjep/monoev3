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
		static string SettingsFileName = "firmwareSettings.xml";
		static FirmwareSettings settings = new FirmwareSettings();
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
			IEnumerable<MenuItemWithAction> items = Directory.EnumerateFiles("/home/root/apps/", "*.exe")
				.Select( (filename) => new MenuItemWithAction(lcd, GetFileNameWithoutExt(filename),  () => StartApp(filename)));
			Menu m = new Menu(font, lcd, btns, "Run program:", items);
			m.Show();
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
			
			List<MenuItemWithAction> items = new List<MenuItemWithAction>();
			items.Add (new MenuItemWithAction(lcd, "Information", () => Information(lcd, btns),MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "Run programs", () => RunPrograms(lcd, btns),MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "Settings", () => ShowSettings(lcd,btns), MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "Shutdown", () => Shutdown(lcd,btns)));
			Menu m = new Menu(font, lcd, btns ,"Main menu", items);
			m.Show();
		}
		
		static bool ShowSettings (Lcd lcd, Buttons btns)
		{
			//Create the settings items and apply the settings 
			List<IMenutem> items = new List<IMenutem> ();
			var debugItem = new MenuItemWithCheckBox (lcd, "Remote debug", settings.DebugMode);
			var debugPortItem = new MenuItemWithNumericInput(lcd, "Debug port",settings.DebugPort,1, ushort.MaxValue);
			items.Add (debugItem);
			items.Add (debugPortItem);
			
			//Show the menu
			Menu m = new Menu (font, lcd, btns, "Settings", items);
			m.Show ();
			
			//Save the new settings
			FirmwareSettings newXmlSettings = new FirmwareSettings();
			newXmlSettings.DebugMode = debugItem.Checked;
			newXmlSettings.DebugPort = debugPortItem.Value;
			try{
				newXmlSettings.SaveToXML(SettingsFileName);
			}
			catch
			{
				Console.WriteLine ("Failed to save settings");	
			}
			settings = newXmlSettings;//apply the settings
			return false;
		}
		
		
		public static void Main (string[] args)
		{
			/****** JIT work-around remove when JIT problem is fixed *****/
			System.Threading.Thread.Sleep(10);
			Console.WriteLine("JIT workaround - please remove!!!");
			/**********/
			  
			//Load settings
			try {
				settings = settings.LoadFromXML (SettingsFileName);
			} 
			catch 
			{
				Console.WriteLine ("Failed to read settings. Using default settings");
			}
			
			using (Lcd lcd = new Lcd())
				using (Buttons btns = new Buttons())
				{					
					lcd.DrawBitmap(monoLogo, new Point((int)(Lcd.Width-monoLogo.Width)/2,0));					
					string iptext = "IP: " + GetIpAddress();
					Point textPos = new Point((Lcd.Width-font.TextSize(iptext).X)/2, Lcd.Height-23);
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
