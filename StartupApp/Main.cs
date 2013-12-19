using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;
using System.Diagnostics;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Menus;
using MonoBrickFirmware.Dialogs;

using System.Reflection;
using System.Collections.Generic;
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
			lcd.WriteText(font, startPos+offset*0, "Firmware: 0.1.0.0", true);
			lcd.WriteText(font, startPos+offset*1, "Mono version: " + monoVersion.Substring(0,7), true);
			lcd.WriteText(font, startPos+offset*2, "Mono CLR: " + monoCLR, true);			
			lcd.Update();
			btns.GetKeypress();
			return false;
		}
		
		static bool ShowProgramOptions (string filename, Lcd lcd, Buttons btns)
		{
			string runString = "Run Program";
			string runInDebugString = "Debug Program";
			string deleteString = "Delete Program";
			var dialog = new SelectDialog<string> (Font.MediumFont, lcd, btns, new string[] {
				runString,
				runInDebugString,
				deleteString
			}, "Options", true);
			dialog.Show ();
			if (!dialog.EscPressed) {
				string selection = dialog.GetSelection ();
				if (selection == runString) {
					MenuAction = () => RunAndWaitForProgram (filename, false);
				}
				if (selection == runInDebugString) {
					MenuAction = () => RunAndWaitForProgram (filename, true);
				}
				if (selection == deleteString) {
					var infoDialog = new InfoDialog (font, lcd, btns, "Deleting File. Please wait", false, "Deleting File");
					infoDialog.Show ();
					//if (RunAndWaitForProcess ("rm", filename) == 0) {
					if (RunAndWaitForProcess ("rm", "sdaf") == 0) {
						infoDialog = new InfoDialog (font, lcd, btns, "Program deleted", true, "Deleting File");
					} 
					else 
					{
						infoDialog = new InfoDialog (font, lcd, btns, "Error deleting program", true, "Deleting File");
					}
					infoDialog.Show ();
				}
			} 
			else 
			{
				return false;
			}
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
				.Select( (filename) => new MenuItemWithAction(lcd, GetFileNameWithoutExt(filename),  () => ShowProgramOptions(filename, lcd, btns)));
			Menu m = new Menu(font, lcd, btns, "Run program:", items);
			m.Show();
			return true;
		}
		
		static void RunAndWaitForProgram (string programName, bool runInDebugMode)
		{
			if (runInDebugMode) {
				programName = @"--debug --debugger-agent=transport=dt_socket,address=0.0.0.0:" + settings.DebugPort + ",server=y " + programName;
				using (Lcd lcd = new Lcd ())
				using (Buttons btns = new Buttons ()) {
					System.Diagnostics.Process proc = new System.Diagnostics.Process ();
					Dialog dialog = null;
					Thread escapeThread = null;
					CancellationTokenSource cts = new CancellationTokenSource();
					CancellationToken token = cts.Token;
					string portString  = ("Port: " + settings.DebugPort).PadRight(6).PadRight(6);
					if (settings.TerminateDebugWithEscape) {
						escapeThread = new System.Threading.Thread (delegate() {
							while (!token.IsCancellationRequested) {
								if (btns.GetKeypress (token) == Buttons.ButtonStates.Escape) {
									proc.Kill ();
									Console.WriteLine ("Killing process");
									cts.Cancel();
								}
							}
						});
						escapeThread.Start();
						dialog = new InfoDialog (Font.MediumFont, lcd, btns, portString + " Press escape to terminate", false, "Debug Mode");
					} 
					else 
					{
						dialog = new InfoDialog (Font.MediumFont, lcd, btns, portString + " Waiting for connection.", false, "Debug Mode");	
					}
					dialog.Show ();
					proc.EnableRaisingEvents = false; 
					Console.WriteLine ("Starting process: {0} with arguments: {1}", "/usr/local/bin/mono", programName);
					proc.StartInfo.FileName = "/usr/local/bin/mono";
					proc.StartInfo.Arguments = programName;
					proc.Start ();
					proc.WaitForExit ();
					if (escapeThread != null && !token.IsCancellationRequested) {
						cts.Cancel();
						escapeThread.Join();
					}
				}
			} 
			else 
			{
				RunAndWaitForProcess("/usr/local/bin/mono", programName); 
			}
		}
		
		static int RunAndWaitForProcess(string fileName, string arguments = ""){
			Process proc = new System.Diagnostics.Process ();
			proc.EnableRaisingEvents = false; 
			Console.WriteLine ("Starting process: {0} with arguments: {1}", fileName, arguments);
			proc.StartInfo.FileName = fileName;
			proc.StartInfo.Arguments = arguments;
			proc.Start ();
			proc.WaitForExit ();
			return proc.ExitCode;	
		}
		
		static bool Shutdown (Lcd lcd, Buttons btns)
		{
			var dialog = new QuestionDialog (Font.MediumFont, lcd, btns, "Are you sure?", "Shutdown EV3");
			dialog.Show ();
			if (dialog.IsPositiveSelected) {
				lcd.Clear();
				lcd.WriteText(font, new Point(0,0), "Shutting down...", true);
				lcd.Update();
			
				using (UnixDevice dev = new UnixDevice("/dev/lms_power"))
				{
					dev.IoCtl(0, new byte[0]);
				}
				btns.LedPattern(2);
				MenuAction = () => RunAndWaitForProcess("/lejos/bin/exit", "");			
				return true;
			
			} 
			return false;
		}
		
		static void ShowMainMenu(Lcd lcd, Buttons btns)
		{
			
			List<IMenuItem> items = new List<IMenuItem>();
			items.Add (new MenuItemWithAction(lcd, "Programs", () => RunPrograms(lcd, btns),MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "Debug settings", () => ShowDebugSettings(lcd,btns), MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "Information", () => Information(lcd, btns),MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "Shutdown", () => Shutdown(lcd,btns)));
			items.Add (new MenuItemWithAction(lcd, "Test", () => ShowTest(lcd,btns)));
			Menu m = new Menu(font, lcd, btns ,"Main menu", items);
			m.Show();
		}
		
		static bool ShowTest (Lcd lcd, Buttons btns)
		{
			var dialog = new CharacterDialog(lcd, btns, "Enter number");
			dialog.Show();
			Console.WriteLine(dialog.GetUserInput());
			return false;
		}
		
		static bool ShowDebugSettings (Lcd lcd, Buttons btns)
		{
			//Create the settings items and apply the settings 
			List<IMenuItem> items = new List<IMenuItem> ();
			var terminateWithEscapeItem = new MenuItemWithCheckBox(lcd, "Esc. termination",settings.TerminateDebugWithEscape);
			var debugPortItem = new MenuItemWithNumericInput(lcd, "Debug port",settings.DebugPort,1, ushort.MaxValue);
			
			items.Add(terminateWithEscapeItem);
			items.Add (debugPortItem);
			
			//Show the menu
			Menu m = new Menu (font, lcd, btns, "Debug Settings", items);
			m.Show ();
			
			//Show dialog
			InfoDialog dialog = new InfoDialog(font, lcd,btns,"Saving settings.", false);
			dialog.Show();
			System.Threading.Thread.Sleep(400);
			//Save the new settings
			FirmwareSettings newXmlSettings = new FirmwareSettings();
			newXmlSettings.TerminateDebugWithEscape = terminateWithEscapeItem.Checked; 
			newXmlSettings.DebugPort = debugPortItem.Value;
			
			try{
				newXmlSettings.SaveToXML(SettingsFileName);
				dialog.UpdateMessage("Done!");
				System.Threading.Thread.Sleep(400);
			}
			catch
			{
				dialog = new InfoDialog(font, lcd,btns,"Failed to save settings!", true);
				dialog.Show();
			}
			settings = newXmlSettings;//apply the settings
			return false;
		}
		
		
		public static void Main (string[] args)
		{
			/*// JIT work-around remove when JIT problem is fixed
			System.Threading.Thread.Sleep(10);
			Console.WriteLine("JIT workaround - please remove!!!");
			// JIT work-around
			  
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
				}*/
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
