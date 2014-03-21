using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Display.Menus;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.Settings;
using MonoBrickFirmware.Services;

using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace StartupApp
{
	class MainClass
	{
		static Bitmap monoLogo = Bitmap.FromResouce(Assembly.GetExecutingAssembly(), "monologo.bitmap");
		static Font font = Font.MediumFont;
		static WebServer webServer = null;
		static Action MenuAction = null;
		static string WpaSupplicantFileName = "/mnt/bootpar/wpa_supplicant.conf";
		static string ProgramPathSdCard = "/mnt/bootpar/apps";
		static string ProgramPathEV3 = "/home/root/apps/";
		static FirmwareSettings settings = new FirmwareSettings();
		static string versionString = "Firmware: 0.1.0.0";
		static string versionURL = "http://www.monobrick.dk/MonoBrickFirmwareRelease/latest/version.txt";
		enum ExecutionMode {Normal = 0, Debug = 1, AOT = 2  };
		
		
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
			lcd.WriteText(font, startPos+offset*0, versionString, true);
			lcd.WriteText(font, startPos+offset*1, "Mono version: " + monoVersion.Substring(0,7), true);
			lcd.WriteText(font, startPos+offset*2, "Mono CLR: " + monoCLR, true);			
			lcd.WriteText(font, startPos+offset*3, "IP: " + WiFiDevice.GetIpAddress(), true);			
			lcd.Update();
			btns.GetKeypress();
			return false;
		}
		
		public static bool AOTCompileAndShowDialog(Font font, Lcd lcd, Buttons btns, string filename)
		{
			var infoDialog = new InfoDialog (font, lcd, btns, "AOT Compiling. Please wait", false, "AOT Compile");
			infoDialog.Show ();
			if (AOTCompiling.Compile (filename)) 
			{
				infoDialog = new InfoDialog (font, lcd, btns, "Program AOT compiled", true, "AOT Compile");
				return true;
			}
			else
			{
				infoDialog = new InfoDialog (font, lcd, btns, "Failed to AOT compile", true, "AOT Compile");
				return false;
			}	
		}		
		
		
		static bool ShowProgramOptions (string filename, Lcd lcd, Buttons btns)
		{
			var dialog = new SelectDialog<string> (Font.MediumFont, lcd, btns, new string[] {
				"Run Program",
				"Debug Program",
				"Run In AOT",
				"AOT Compile",
				"Delete Program",
			}, "Options", true);
			dialog.Show ();
			if (!dialog.EscPressed) {
				switch (dialog.GetSelectionIndex ()) {
				case 0:
					lcd.Clear ();
					lcd.DrawBitmap (monoLogo, new Point ((int)(Lcd.Width - monoLogo.Width) / 2, 5));					
					Rectangle textRect = new Rectangle (new Point (0, Lcd.Height - (int)Font.SmallFont.maxHeight - 2), new Point (Lcd.Width, Lcd.Height - 2));
					lcd.WriteTextBox (Font.SmallFont, textRect, "Running...", true, Lcd.Alignment.Center);
					lcd.Update ();						
					MenuAction = () => RunAndWaitForProgram (filename, ExecutionMode.Normal);	
					break;
				case 1:
					MenuAction = () => RunAndWaitForProgram (filename, ExecutionMode.Debug);
					break;
				case 2:
					if (!AOTCompiling.IsFileCompiled (filename)) {
						if (AOTCompileAndShowDialog (font, lcd, btns, filename)) {
							MenuAction = () => RunAndWaitForProgram (filename, ExecutionMode.AOT);	
						}
					} 
					else 
					{
						MenuAction = () => RunAndWaitForProgram (filename, ExecutionMode.AOT);
					}
					break;
				case 3:
					if (AOTCompiling.IsFileCompiled (filename)) {
						var questionDialog = new QuestionDialog(font,lcd,btns,"Progran already compiled. Recompile?","AOT recompile");
						questionDialog.Show();
						if(questionDialog.IsPositiveSelected)
								AOTCompileAndShowDialog(font,lcd,btns,filename);
					} 
					else 
					{
						AOTCompileAndShowDialog(font,lcd,btns,filename);
					}
					break;
				case 4:
					var infoDialog = new InfoDialog (font, lcd, btns, "Deleting File. Please wait", false, "Deleting File");
					infoDialog.Show ();
					if (ProcessHelper.RunAndWaitForProcess ("rm", filename) == 0) {
							
					} 
					else 
					{
						infoDialog = new InfoDialog (font, lcd, btns, "Error deleting program", true, "Deleting File");
					}
					infoDialog.Show ();
					break;
				}
				return true;
			} 
			return false;
			
		}
		
		static string GetFileNameWithoutExt(string fullname)
		{
			string filename = new FileInfo(fullname).Name;
			return filename.Substring(0, filename.Length-4);
		}
		
		static bool RunPrograms (Lcd lcd, Buttons btns)
		{
			IEnumerable<MenuItemWithAction> itemsFromEV3 = Directory.EnumerateFiles (ProgramPathEV3, "*.exe")
				.Select ((filename) => new MenuItemWithAction (lcd, GetFileNameWithoutExt (filename), () => ShowProgramOptions (filename, lcd, btns)));
			IEnumerable<MenuItemWithAction> itemsFromSD = Directory.EnumerateFiles (ProgramPathSdCard, "*.exe")
				.Select ((filename) => new MenuItemWithAction (lcd, GetFileNameWithoutExt (filename), () => ShowProgramOptions (filename, lcd, btns)));
			Menu m = new Menu(font, lcd, btns, "Run program:", itemsFromEV3.Concat(itemsFromSD));
			m.Show();
			return true;
		}
		
		static void RunAndWaitForProgram (string programName, ExecutionMode mode)
		{
			switch (mode) 
			{
				case ExecutionMode.Debug:
					programName = @"--debug --debugger-agent=transport=dt_socket,address=0.0.0.0:" + settings.DebugSettings.Port + ",server=y " + programName;
					using (Lcd lcd = new Lcd ())
					using (Buttons btns = new Buttons ()) {
						System.Diagnostics.Process proc = new System.Diagnostics.Process ();
						Dialog dialog = null;
						Thread escapeThread = null;
						CancellationTokenSource cts = new CancellationTokenSource();
						CancellationToken token = cts.Token;
						string portString  = ("Port: " + settings.DebugSettings.Port).PadRight(6).PadRight(6);
						if (settings.DebugSettings.TerminateWithEscape) {
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
					break;
				case ExecutionMode.Normal:
					ProcessHelper.RunAndWaitForProcess("/usr/local/bin/mono", programName); 
					break;
				case ExecutionMode.AOT:
					ProcessHelper.RunAndWaitForProcess("/usr/local/bin/mono --full-aot", programName);	
					break;
			}
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
				MenuAction = () => ProcessHelper.RunAndWaitForProcess("/lejos/bin/exit");			
				return true;
			
			} 
			return false;
		}
		
		static void ShowMainMenu(Lcd lcd, Buttons btns)
		{
			
			List<IMenuItem> items = new List<IMenuItem>();
			items.Add (new MenuItemWithAction(lcd, "Programs", () => RunPrograms(lcd, btns),MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "WiFi Connection", () => ShowWiFiMenu(lcd,btns), MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "WebServer", () => ShowWebServerMenu(lcd,btns), MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "Settings", () => ShowSettings(lcd,btns), MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "Information", () => Information(lcd, btns)));
			items.Add (new MenuItemWithAction(lcd, "Check for Updates", () => ShowUpdatesDialogs(lcd,btns,true)));
			items.Add (new MenuItemWithAction(lcd, "Shutdown", () => Shutdown(lcd,btns)));
			Menu m = new Menu(font, lcd, btns ,"Main menu", items);
			m.Show();
		}
		
		static bool ShowWebServerMenu (Lcd lcd, Buttons btns)
		{
			List<IMenuItem> items = new List<IMenuItem> ();
			var portItem = new MenuItemWithNumericInput(lcd,"Port", settings.WebServerSettings.Port, 1, ushort.MaxValue);
			portItem.OnValueChanged+= delegate(int value) 
			{
				new Thread(delegate() {
			    	settings.WebServerSettings.Port = value;
					settings.Save();
				}).Start();
			};
			var startItem = new MenuItemWithCheckBox(lcd,"Start server", WebServer.IsRunning(),
				delegate(bool running)
       	 		{ 
					webServer = new WebServer(settings.WebServerSettings.Port);
					bool isRunning = running;
					if(running){
						var dialog = new InfoDialog(font,lcd,btns,"Shutting down Web-Server", false);
						dialog.Show();
						webServer.Stop();
						dialog = new InfoDialog(font,lcd,btns,"Web-server Stopped!", true);
						dialog.Show();
						isRunning = false;
					}
					else{
						var dialog = new InfoDialog(font,lcd,btns,"Starting Web-Server Please Wait", false,"Web-Server");
						dialog.Show();
			            webServer.CompilingServer += delegate() { dialog.UpdateMessage("Compiling...");};
			            webServer.LoadingPage += delegate() {dialog.UpdateMessage("Loading page"); };
			            webServer.StartingServer += delegate() {dialog.UpdateMessage("Starting server");};
			            if(webServer.Restart()){
							dialog = new InfoDialog(font,lcd,btns,"Started successfully at port" + settings.WebServerSettings.Port, true);
							dialog.Show();
							isRunning = true;
						}
						else{
							dialog = new InfoDialog(font,lcd,btns,"Failed to start Web-Servers", true);
							dialog.Show();
							isRunning = false;
						}
					}
					return isRunning;
       			} 
			);
			
			items.Add(portItem);
			items.Add(startItem);
			//Show the menu
			Menu m = new Menu (font, lcd, btns, "Web Server", items);
			m.Show ();
			return false;
		}
		
		static bool ShowWiFiMenu (Lcd lcd, Buttons btns)
		{
			List<IMenuItem> items = new List<IMenuItem> ();
			var ssidItem = new MenuItemWithCharacterInput(lcd,btns,"SSID", "Enter SSID", settings.WiFiSettings.SSID);
			ssidItem.OnDialogExit += delegate(string text) {
				new Thread(delegate() {
		    		settings.WiFiSettings.SSID = text;
					settings.Save();
			    }).Start();
			};
			var passwordItem = new MenuItemWithCharacterInput(lcd,btns,"Password", "Password", settings.WiFiSettings.Password, true);
			passwordItem.OnDialogExit += delegate(string text) {
				new Thread(delegate() {
			    	settings.WiFiSettings.Password = text;
					settings.Save();
			    }).Start();
			};
			var encryptionItem = new MenuItemWithOptions<string>(lcd,"Encryption", new string[]{"None","WPA/2"}, settings.WiFiSettings.Encryption ? 1 : 0);
			encryptionItem.OnOptionChanged += delegate(string newOpstion) {
				new Thread(delegate() {
		    		if(newOpstion == "None")
						settings.WiFiSettings.Encryption = false;
					else
						settings.WiFiSettings.Encryption = true;
					settings.Save(); 
			    }).Start();
			};
			var connectItem = new MenuItemWithCheckBox(lcd,"Connect", WiFiDevice.IsLinkUp(),
				delegate(bool WiFiOn)
         		{ 
					bool isOn = WiFiOn;
					var infoDialog = new InfoDialog(font,lcd,btns,"Creating Configuration file", false);
					infoDialog.Show();
					WriteWpaSupplicantConfiguration(settings.WiFiSettings.SSID,settings.WiFiSettings.Password,settings.WiFiSettings.Encryption);
					if(WiFiOn){
						var dialog = new InfoDialog(font,lcd,btns,"Shutting down WiFi", false);
						dialog.Show();
						WiFiDevice.TurnOff();
						dialog = new InfoDialog(font,lcd,btns,"WiFi Disabled!!", true);
						dialog.Show();
						isOn = false;
					}
					else{
						var dialog = new InfoDialog(font,lcd,btns,"Connecting to WiFi Network Please Wait", false);
						dialog.Show();
						if(WiFiDevice.TurnOn(60000)){
							if(settings.WiFiSettings.ConnectAtStartUp == false){
								var question = new QuestionDialog(font,lcd,btns,"Do you want to connect at start-up?", "Settings");
								question.Show();
								if(question.IsPositiveSelected){
									new Thread(delegate() {
								    	settings.WiFiSettings.ConnectAtStartUp = true;
										settings.Save();
								    }).Start();
								}
							}
							dialog = new InfoDialog(font,lcd,btns,"Connected Successfully " + WiFiDevice.GetIpAddress(), true);
							dialog.Show();
							isOn = true;
						}
						else{
							dialog = new InfoDialog(font,lcd,btns,"Failed to connect to WiFI Network", true);
							dialog.Show();
							isOn = false;
						}
					}
					return isOn;
         		} 
			);
			items.Add(ssidItem);
			items.Add(passwordItem);
			items.Add(encryptionItem);
			items.Add(connectItem);
			//Show the menu
			Menu m = new Menu (font, lcd, btns, "WiFi Connection", items);
			m.Show ();
			return false;	
		}

		
		static bool ShowSettings (Lcd lcd, Buttons btns)
		{
			//Create the settings items and apply the settings 
			List<IMenuItem> items = new List<IMenuItem> ();
			var terminateWithEscapeItem = new MenuItemWithCheckBox(lcd, "Debug termination",settings.DebugSettings.TerminateWithEscape);
			var debugPortItem = new MenuItemWithNumericInput(lcd, "Debug port",settings.DebugSettings.Port,1, ushort.MaxValue);
			var checkForUpdate = new MenuItemWithCheckBox(lcd, "Update check",settings.GeneralSettings.CheckForSwUpdatesAtStartUp);
			var wifiConnect = new MenuItemWithCheckBox(lcd, "WiFi auto connect",settings.WiFiSettings.ConnectAtStartUp);
			var soundVolume = new MenuItemWithNumericInput(lcd, "Volume",settings.SoundSettings.Volume);
			var enableSound = new MenuItemWithCheckBox(lcd, "Enable sound", settings.SoundSettings.EnableSound);
			
			
			items.Add(wifiConnect);
			items.Add(checkForUpdate);
			items.Add(terminateWithEscapeItem);
			items.Add (debugPortItem);
			items.Add(soundVolume);
			
			//Show the menu
			Menu m = new Menu (font, lcd, btns, "Settings", items);
			m.Show ();
			new Thread(delegate() {
	    		settings.DebugSettings.TerminateWithEscape = terminateWithEscapeItem.Checked; 
				settings.DebugSettings.Port = debugPortItem.Value;
				settings.GeneralSettings.CheckForSwUpdatesAtStartUp = checkForUpdate.Checked;
				settings.WiFiSettings.ConnectAtStartUp = wifiConnect.Checked;
				settings.SoundSettings.Volume = soundVolume.Value;
				settings.SoundSettings.EnableSound = enableSound.Checked;
				settings.Save();
			}).Start();
			return false;
		}
		
		
		public static void WriteWpaSupplicantConfiguration (string ssid, string password, bool useEncryption)
		{
			MonoBrickFirmware.Native.ProcessHelper.RunAndWaitForProcess("rm",WpaSupplicantFileName); 
			string encryption;
			if(useEncryption)
				encryption = "WPA-PSK";
			else
				encryption = "NONE";	             
			string[] lines = { 
				"#This file is auto generated by MonoBrick - will be overwritten at startup", 
				"#ap_scan=1", 
				"#fast_reauth=1",
				"# Pers", 
				"ctrl_interface=/var/run/wpa_supplicant", 
				"network={", 
				"  ssid=\"" + ssid + "\"",
				"  key_mgmt="+encryption, 
				"  psk=\"" + password + "\"", 
				"  pairwise=CCMP TKIP",
				"  group=CCMP TKIP", 
				"}", 
			};
      System.IO.File.WriteAllLines(@WpaSupplicantFileName, lines);
		}
		
		static bool ShowUpdatesDialogs (Lcd lcd, Buttons btns, bool showDescriptionDialog)
		{
			if (WiFiDevice.IsLinkUp ()) {
				try {
					InfoDialog dialog = null;
					if(showDescriptionDialog){
						dialog = new InfoDialog(font, lcd, btns, "Checking for updates. Please wait",false);
						dialog.Show();
					}
					if (UpdateAvailable ()) {
						dialog = new InfoDialog (font, lcd, btns, "Software update available. Visit monobrick.dk", true);
					} else {
						dialog = new InfoDialog (font, lcd, btns, "No software updates available", true);
					}
					dialog.Show ();
				} 
				catch {
					InfoDialog dialog = new InfoDialog (font, lcd, btns, "Failed to check for updates", true);
					dialog.Show ();		
				}
			} 
			else 
			{
				var dialog = new InfoDialog (font, lcd, btns, "WiFi device is not pressent", true);
				dialog.Show();	
			}
			return false;
		}
		
		
		public static bool UpdateAvailable ()
		{
				var textFromFile = (new WebClient ()).DownloadString (versionURL).TrimEnd (new char[] { '\r', '\n' });
				return versionString != textFromFile;
		}
		
		
		public static void Main (string[] args)
		{
			using (Lcd lcd = new Lcd ())
			using (Buttons btns = new Buttons ()) {					
				lcd.DrawBitmap (monoLogo, new Point ((int)(Lcd.Width - monoLogo.Width) / 2, 5));					
				Rectangle textRect = new Rectangle (new Point (0, Lcd.Height - (int)Font.SmallFont.maxHeight - 2), new Point (Lcd.Width, Lcd.Height - 2));
				
				lcd.WriteTextBox (Font.SmallFont, textRect, "Initializing...", true, Lcd.Alignment.Center);
				lcd.Update ();						
				WiFiDevice.TurnOff ();
				if (!Directory.Exists (ProgramPathSdCard))
					Directory.CreateDirectory (ProgramPathSdCard);
				
				// JIT work-around remove when JIT problem is fixed
				System.Threading.Thread.Sleep (10);
				Console.WriteLine ("JIT workaround - please remove!!!");
				lcd.WriteTextBox (Font.SmallFont, textRect, "Checking WiFi...", true, Lcd.Alignment.Center);
				lcd.Update ();						
				//WiFiDevice.IsLinkUp ();
				lcd.WriteTextBox (Font.SmallFont, textRect, "Starting Mono Runtime...", true, Lcd.Alignment.Center);
				lcd.Update ();						
				string monoVersion = "Unknown";
				Type type = Type.GetType ("Mono.Runtime");
				if (type != null) {                                          
					MethodInfo displayName = type.GetMethod ("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static); 
					if (displayName != null)
						monoVersion = (string)displayName.Invoke (null, null);
					Console.WriteLine ("Mono Version" + monoVersion); 
				}	
				string monoCLR = System.Reflection.Assembly.GetExecutingAssembly ().ImageRuntimeVersion;
				// JIT work-around end but look for more below
				
				//Load settings
				lcd.WriteTextBox (Font.SmallFont, textRect, "Loading settings...", true, Lcd.Alignment.Center);
				lcd.Update ();
				settings = settings.Load();						
				if (settings != null) {
					lcd.WriteTextBox (Font.SmallFont, textRect, "Applying settings...", true, Lcd.Alignment.Center);
					lcd.Update ();						
					settings.Save();// JIT work-around
          			WriteWpaSupplicantConfiguration(settings.WiFiSettings.SSID, settings.WiFiSettings.Password, settings.WiFiSettings.Encryption);
					if (settings.WiFiSettings.ConnectAtStartUp) {
						lcd.WriteTextBox (Font.SmallFont, textRect, "Connecting to WiFi...", true, Lcd.Alignment.Center);
						lcd.Update ();						
						if (WiFiDevice.TurnOn (60000)) {
							WiFiDevice.GetIpAddress ();// JIT work-around
              				if (settings.GeneralSettings.CheckForSwUpdatesAtStartUp)
              				{
								ShowUpdatesDialogs (lcd, btns, false);
							} 
							else 
							{
								var dialog = new InfoDialog (font, lcd, btns, "Connected Successfully " + WiFiDevice.GetIpAddress (), true);
								dialog.Show ();
							} 
						} 
						else
						{
							var dialog = new InfoDialog (font, lcd, btns, "Failed to connect to WiFI Network", true);
							dialog.Show ();
						}
					}
				} 
				else 
				{
					var dialog = new InfoDialog (font, lcd, btns, "Failed to load settings", true);
					dialog.Show ();
          			settings = new FirmwareSettings();
				}
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
						
	namespace Extensions
	{
		public static class AOTExtension
	    {
	        
						
	    }
	}
	
	
}
