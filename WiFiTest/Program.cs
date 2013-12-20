using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using MonoBrickFirmware.Menus;
using MonoBrickFirmware.Dialogs;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace WiFiTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (Lcd lcd = new Lcd())
				using (Buttons btns = new Buttons())
				{
					ShowMenu(lcd, btns);					
				}			
		}
		
		static void ShowMenu(Lcd lcd, Buttons btns)
		{
			
			List<IMenuItem> items = new List<IMenuItem>();
			items.Add (new MenuItemWithAction(lcd, "Turn On Wlan", () =>TurnOnWlan(lcd,btns)));
			items.Add (new MenuItemWithAction(lcd, "Turn Off Wlan",  () =>TurnOffWlan(lcd,btns)));
			items.Add (new MenuItemWithAction(lcd, "Print IP Address", () => PrintIpAddress(lcd,btns)));
			items.Add (new MenuItemWithAction(lcd, "Check link", () => CheckLinkUp(lcd,btns)));
			items.Add (new MenuItemWithAction(lcd, "Gateway", () => PrintGateWay(lcd,btns)));
			items.Add (new MenuItemWithAction(lcd, "Stop", delegate {return true;} ));
			Menu m = new Menu(Font.MediumFont, lcd, btns ,"Menu", items);
			m.Show();
		}
		
		
		static bool PrintGateWay(Lcd lcd, Buttons btns)
		{
			string s = 	WiFiDevice.Gateway();
			string[] results = s.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
			foreach(var myString in results)
				LcdConsole.WriteLine(myString);
			btns.GetKeypress();
			return false;
		}
		
		
		
		static bool TurnOffWlan (Lcd lcd, Buttons btns)
		{
			ProcessHelper.RunAndWaitForProcess("killall","wpa_supplicant");
			return false;
		}
		
		static bool TurnOnWlan (Lcd lcd, Buttons btns)
		{	
			string s;
			if (WiFiDevice.TurnOn(30000)) {
				s = "WiFi is turned on";
			} 
			else 
			{
				s = "WiFi is not turned on";
			}
			var dialog = new MonoBrickFirmware.Dialogs.InfoDialog(Font.MediumFont,lcd,btns,s,true,"Turn On");
			dialog.Show();
			return false;
		}
		
		static bool PrintIpAddress(Lcd lcd, Buttons btns)
		{
			var dialog = new MonoBrickFirmware.Dialogs.InfoDialog(Font.MediumFont,lcd,btns,GetIpAddress(),true,"IP Address");
			dialog.Show();
			return false;
		}
		
		static bool CheckLinkUp (Lcd lcd, Buttons btns)
		{
			string s;
			if (WiFiDevice.IsLinkUp()) {
				s = "Link is up";
			} 
			else 
			{
				s = "Link is down";
			}
			var dialog = new MonoBrickFirmware.Dialogs.InfoDialog(Font.MediumFont,lcd,btns,s,true,"Wlan Status");
			dialog.Show();
			return false;
		}
		
		static bool ShowIpAddress(Lcd lcd, Buttons btns)
		{
			var dialog = new MonoBrickFirmware.Dialogs.InfoDialog(Font.MediumFont,lcd,btns,GetIpAddress (),true,"IP Address");
			dialog.Show();
			return false;

		}
		
		
		public static string GetIpAddress ()
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
	}
}
