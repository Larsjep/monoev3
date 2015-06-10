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
using MonoBrickFirmware.Connections;
using MonoBrickFirmware.Tools;
using MonoBrickFirmware.FileSystem;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Nancy;
using Nancy.Hosting.Self;

namespace StartupApp
{
	class MainClass
	{
		static Bitmap monoLogo = Bitmap.FromResouce(Assembly.GetExecutingAssembly(), "monologo.bitmap");
		static FirmwareSettings settings = new FirmwareSettings();
		static string WpaSupplicantFileName = "/mnt/bootpar/wpa_supplicant.conf";
		static string firmwareVersion= "1.0.0.0";

		static bool updateProgramList = false;
		
		#region Main Menu
		static void ShowMainMenu()
		{
			
			List<IChildItem> items = new List<IChildItem>();
			items.Add (new ItemWithAction("Programs", () => RunPrograms(),ItemSymbole.RightArrow));
			items.Add (new ItemWithAction("WiFi Connection", () => ShowWiFiMenu(), ItemSymbole.RightArrow));
			items.Add (new ItemWithAction("WebServer", () => ShowWebServerMenu(), ItemSymbole.RightArrow));
			items.Add (new ItemWithAction("Settings", () => ShowSettings(), ItemSymbole.RightArrow));
			items.Add (new ItemWithAction("Information", () => Information()));
			items.Add (new ItemWithAction("Check for Updates", () => ShowUpdatesDialogs()));
			items.Add (new ItemWithAction("Shutdown", () => Shutdown()));
			Menu m = new Menu("Main menu", items);
			m.Show();
		}
		#endregion


		#region WebServer Menu
		/*static bool ShowWebServerMenu ()
		{
			List<IChildItem> items = new List<IChildItem> ();
			var portItem = new ItemWithNumericInput("Port", settings.WebServerSettings.Port, 1, ushort.MaxValue);
			portItem.OnValueChanged+= delegate(int value) 
			{
				new Thread(delegate() {
			    	settings.WebServerSettings.Port = value;
					settings.Save();
				}).Start();
			};
			var startItem = new ItemWithCheckBox("Start server", Webserver.Instance.IsRunning,
				delegate(bool running)
       	 		{ 
					
					bool isRunning = running;
					if(running){
						var step = new StepContainer(
							delegate() 
							{
								Webserver.Instance.Stop();
								return true;
							},
							"Stopping", "Failed to stop");
						var dialog = new ProgressDialog("Web Server",step);
						dialog.Show();
						isRunning = Webserver.Instance.IsRunning;
					}
					else{
						var step1 = new StepContainer(()=>{Webserver.Instance.Start(portItem.Value); return true;}, "Starting REST", "Failed To Start REST");
						var step2 = new StepContainer(()=>{return Webserver.Instance.LoadPage();}, "Loading Webpage", "Failed to load page");
						var stepDialog = new StepDialog("Web Server", new List<IStep>{step1,step2}, "Webserver started");
						isRunning = stepDialog.Show();
					}
					return isRunning;
       			} 
			);
			
			//items.Add(portItem);
			items.Add(startItem);
			//Show the menu
			Menu m = new Menu ("Web Server", items);
			m.Show ();
			return false;
		}*/
		#endregion
		

		
		#region Information Menu
		static bool Information()
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
			
			Point offset = new Point(0, (int)Font.MediumFont.maxHeight);
			Point startPos = new Point(0,0);
			Lcd.Instance.Clear();
			Lcd.Instance.WriteText(Font.MediumFont, startPos+offset*0, "Firmware: " + firmwareVersion, true);
			Lcd.Instance.WriteText(Font.MediumFont, startPos+offset*1, "Image: " + VersionHelper.CurrentImageVersion(), true);
			Lcd.Instance.WriteText(Font.MediumFont, startPos+offset*2, "Mono version: " + monoVersion.Substring(0,7), true);
			Lcd.Instance.WriteText(Font.MediumFont, startPos+offset*3, "Mono CLR: " + monoCLR, true);			
			Lcd.Instance.WriteText(Font.MediumFont, startPos+offset*4, "IP: " + WiFiDevice.GetIpAddress(), true);			

			Lcd.Instance.Update();
			Buttons.Instance.GetKeypress();
			return false;
		}
		#endregion
		
		#region Update Menu
		static bool ShowUpdatesDialogs ()
		{
			if (WiFiDevice.IsLinkUp ()) {
				bool newImage = false;
				bool newFirmwareApp = false;
				bool newAddin = false;
				VersionInfo versionInfo = null;
				var step = new StepContainer (
					delegate() {
						try {
							versionInfo = VersionHelper.AvailableVersions ();
							newImage = versionInfo.Image != VersionHelper.CurrentImageVersion ();
							newFirmwareApp = versionInfo.Fimrware != firmwareVersion;
							string addInVersion = VersionHelper.CurrentAddInVersion ();
							if (addInVersion != null)
								newAddin = versionInfo.AddIn != VersionHelper.CurrentAddInVersion ();
						} catch {
							return false;
						}
						return true;
					},
					           "Checking server", "Failed to check for Updates"); 
				var dialog = new ProgressDialog ("Updates", step);
				dialog.Show ();
				if (newImage) {
					var visitWebsiteDialog = new InfoDialog ("New image available. Download it at monobrick.dk", true);
					visitWebsiteDialog.Show ();
				} else {
					if (newFirmwareApp) {
						var updateQuestion = new QuestionDialog ("New firmware available. Update?", "New Fiwmware");
						if (updateQuestion.Show ()) {
							var updateHelper = new UpdateHelper (versionInfo.Fimrware);
							List<IStep> steps = new List<IStep> ();
							steps.Add (new StepContainer (updateHelper.DownloadFirmware, "Downloading...", "Failed to download files"));
							steps.Add (new StepContainer (updateHelper.UpdateBootFile, "Updating system", "Failed to update boot file"));
							var updateDialog = new StepDialog ("Updating", steps);
							if (updateDialog.Show ()) {
								for (int seconds = 10; seconds > 0; seconds--) {
									var rebootDialog = new InfoDialog ("Update completed. Rebooting in  " + seconds, false);
									rebootDialog.Show ();
									System.Threading.Thread.Sleep (1000);
								}
								ProcessHelper.RunAndWaitForProcess ("/sbin/shutdown", "-h now");
								Thread.Sleep (120000);
								var whyAreYouHereDialog = new InfoDialog ("Cut the power", false, "Reboot failed");
								whyAreYouHereDialog.Show ();
								new ManualResetEvent (false).WaitOne ();
							}
						}
					} else {
						if (newAddin) {
							var visitWebsiteDialog = new InfoDialog ("New Xamarin Add-in. Download it at monobrick.dk", true);
							visitWebsiteDialog.Show ();	
						} else 
						{
							var noUpdateDialog = new InfoDialog ("No updates available", true);
							noUpdateDialog.Show ();	
						} 
					}
				}
			} 
			else 
			{
				var dialog = new InfoDialog ("WiFi device is not pressent", true);
				dialog.Show();	
			}
			return false;
		}
		#endregion

		#region Main Program

		static internal ManualResetEvent terminateProgram = new ManualResetEvent(false);


		public static void Main (string[] args)
		{
			Lcd.Instance.DrawBitmap (monoLogo, new Point ((int)(Lcd.Width - monoLogo.Width) / 2, 5));					
			Rectangle textRect = new Rectangle (new Point (0, Lcd.Height - (int)Font.SmallFont.maxHeight - 2), new Point (Lcd.Width, Lcd.Height - 2));
			
			Lcd.Instance.WriteTextBox (Font.SmallFont, textRect, "Initializing...", true, Lcd.Alignment.Center);
			Lcd.Instance.Update ();						
			WiFiDevice.TurnOff ();
			ProgramManager.Instance.CreateSDCardFolder();

			// JIT work-around remove when JIT problem is fixed
			System.Threading.Thread.Sleep (10);
			Console.WriteLine ("JIT workaround - please remove!!!");
			Lcd.Instance.WriteTextBox (Font.SmallFont, textRect, "Checking WiFi...", true, Lcd.Alignment.Center);
			Lcd.Instance.Update ();						
			//WiFiDevice.IsLinkUp ();
			Lcd.Instance.WriteTextBox (Font.SmallFont, textRect, "Starting Mono Runtime...", true, Lcd.Alignment.Center);
			Lcd.Instance.Update ();						
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
			Lcd.Instance.WriteTextBox (Font.SmallFont, textRect, "Loading settings...", true, Lcd.Alignment.Center);
			Lcd.Instance.Update ();
			settings = settings.Load();						
			if (settings != null) {
				Lcd.Instance.WriteTextBox (Font.SmallFont, textRect, "Applying settings...", true, Lcd.Alignment.Center);
				Lcd.Instance.Update ();						
				settings.Save();// JIT work-around
      			WriteWpaSupplicantConfiguration(settings.WiFiSettings.SSID, settings.WiFiSettings.Password, settings.WiFiSettings.Encryption);
				if (settings.WiFiSettings.ConnectAtStartUp) {
					Lcd.Instance.WriteTextBox (Font.SmallFont, textRect, "Connecting to WiFi...", true, Lcd.Alignment.Center);
					Lcd.Instance.Update ();						
					if (WiFiDevice.TurnOn (60000)) {
						WiFiDevice.GetIpAddress ();// JIT work-around
          				if (settings.GeneralSettings.CheckForSwUpdatesAtStartUp)
          				{
							ShowUpdatesDialogs ();
						} 
						else 
						{
							var dialog = new InfoDialog ("Connected Successfully " + WiFiDevice.GetIpAddress (), true);
							dialog.Show ();
						} 
					} 
					else
					{
						var dialog = new InfoDialog ("Failed to connect to WiFI Network", true);
						dialog.Show ();
					}
				}
			} 
			else 
			{
				var dialog = new InfoDialog ("Failed to load settings", true);
				dialog.Show ();
      			settings = new FirmwareSettings();
			}
			
			//Keep showing the menu even if user press esc
			while(true)
			{
				ShowMainMenu();
			}
		}
		#endregion
	}
	
}
