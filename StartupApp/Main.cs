using System;
using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Display.Menus;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.Settings;
using MonoBrickFirmware.Connections;
using MonoBrickFirmware.Tools;
using MonoBrickFirmware.FileSystem;
using System.Reflection;
using System.Collections.Generic;

using Nancy;
using Nancy.Hosting.Self;

namespace StartupApp
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			Menu menu = new Menu ("Main Menu");

			menu.AddItem(new ItemWithProgramList ("Programs", false));
			menu.AddItem(new ItemWiFiOptions ());
			menu.AddItem(new ItemWithSettings ());
			menu.AddItem(new ItemWithUpdateDialog ());
			//menu.AddItem(new ItemWithWebserver ());
			menu.AddItem(new ItemWithBrickInfo ());
			menu.AddItem(new ItemWithShutDown ());

			var container = new FirmwareMenuContainer (menu);

			Bitmap monoLogo = Bitmap.FromResouce(Assembly.GetExecutingAssembly(), "monologo.bitmap");
			Lcd.DrawBitmap (monoLogo, new Point ((int)(Lcd.Width - monoLogo.Width) / 2, 5));					
			Rectangle textRect = new Rectangle (new Point (0, Lcd.Height - (int)Font.SmallFont.maxHeight - 2), new Point (Lcd.Width, Lcd.Height - 2));
			
			Lcd.WriteTextBox (Font.SmallFont, textRect, "Initializing...", true, Lcd.Alignment.Center);
			Lcd.Update ();
			WiFiDevice.TurnOff ();
			ProgramManager.CreateSDCardFolder();
			Lcd.WriteTextBox (Font.SmallFont, textRect, "Loading settings...", true, Lcd.Alignment.Center);
			Lcd.Update ();
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
			container.Show ();
		}
	}
	
}
