using MonoBrickFirmware.Display;
using MonoBrickFirmware.Display.Menus;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.Settings;
using MonoBrickFirmware.Connections;
using MonoBrickFirmware.FileSystem;
using System.IO;
using System.Reflection;

namespace StartupApp
{
	public class MainClass
	{

		private static FirmwareMenuContainer container = null;
		public static string SuspendFile = @"/usr/local/bin/suspendFirmware.txt";

		public static void Main (string[] args)
		{
			if (!File.Exists (SuspendFile))
			{
				File.Create (SuspendFile);
			}
			FileSystemWatcher watcher = new FileSystemWatcher();
			watcher.Path = Path.GetDirectoryName(SuspendFile);
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Filter = Path.GetFileName(SuspendFile);
			watcher.Changed += OnSuspendFileChanged;
			watcher.EnableRaisingEvents = true;

			Menu menu = new Menu ("Main Menu");

			menu.AddItem(new ItemWithProgramList ("Programs", false));
			menu.AddItem(new ItemWiFiOptions ());
			menu.AddItem(new ItemWithSettings ());
			menu.AddItem(new ItemWithUpdateDialog ());
			//menu.AddItem(new ItemWithWebserver ());
			menu.AddItem(new ItemWithBrickInfo ());
			menu.AddItem(new ItemWithTurnOff ());

			container = new FirmwareMenuContainer (menu);

			Bitmap monoLogo = Bitmap.FromResouce(Assembly.GetExecutingAssembly(), "monologo.bitmap");
			Lcd.DrawBitmap (monoLogo, new Point ((int)(Lcd.Width - monoLogo.Width) / 2, 5));					
			Rectangle textRect = new Rectangle (new Point (0, Lcd.Height - (int)Font.SmallFont.maxHeight - 2), new Point (Lcd.Width, Lcd.Height - 2));

			Lcd.WriteTextBox (Font.SmallFont, textRect, "Initializing...", true, Lcd.Alignment.Center);
			Lcd.Update ();
			WiFiDevice.TurnOff ();
			ProgramManager.CreateSDCardFolder();
			Lcd.WriteTextBox (Font.SmallFont, textRect, "Loading settings...", true, Lcd.Alignment.Center);
			Lcd.Update ();
			FirmwareSettings.Load ();
			Lcd.WriteTextBox (Font.SmallFont, textRect, "Applying settings...", true, Lcd.Alignment.Center);
			Lcd.Update ();						
			if (FirmwareSettings.GeneralSettings.ConnectToWiFiAtStartUp) 
			{
				Lcd.WriteTextBox (Font.SmallFont, textRect, "Connecting to WiFi...", true, Lcd.Alignment.Center);
				Lcd.Update ();						
				if (WiFiDevice.TurnOn (FirmwareSettings.WiFiSettings.SSID, FirmwareSettings.WiFiSettings.Password, FirmwareSettings.WiFiSettings.Encryption, 40000)) 
				{
					if (FirmwareSettings.GeneralSettings.CheckForSwUpdatesAtStartUp)
					{
						container.Show (3); //show the menu container with the update dialog
						return;
					} 
					else 
					{
						var dialog = new InfoDialog ("Connected Successfully " + WiFiDevice.GetIpAddress ());
						dialog.Show ();
					} 
				} 
				else 
				{
					var dialog = new InfoDialog ("Failed to connect to WiFI Network");
					dialog.Show ();
				}
			}
			container.Show ();	
		}

		private static void OnSuspendFileChanged(object sender, FileSystemEventArgs e)
		{
			if (container != null) 
			{
				if (File.ReadAllText (SuspendFile).Trim() == "1")
				{
					container.SuspendButtonEvents ();
				} 
				else 
				{
					container.ResumeButtonEvents ();
				}
			}
		}

		public static void Kill()
		{
			if (container != null) 
			{
				container.Terminate ();
			}
		}

	}
	
}
