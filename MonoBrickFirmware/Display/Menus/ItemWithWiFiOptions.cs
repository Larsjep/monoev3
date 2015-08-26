using System;
using System.Collections.Generic;
using MonoBrickFirmware.Settings;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.Connections;
using System.Threading;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWiFiOptions : ItemList
	{

		private ItemWithCharacterInput ssidItem;
		private ItemWithCharacterInput passwordItem;
		private ItemWithOptions<string> encryptionItem;
		private TurnWiFiOnOffCheckBox connectItem; 
		public ItemWiFiOptions(): base("WiFi", Font.MediumFont, true)
		{
			
		}

		protected override List<IChildItem> OnCreateChildList ()
		{
			ssidItem = new ItemWithCharacterInput("SSID", "Enter SSID", FirmwareSettings.WiFiSettings.SSID, OnSsidChanged);
			passwordItem = new ItemWithCharacterInput("Password", "Password", FirmwareSettings.WiFiSettings.Password, OnPasswordChanged, true);
			encryptionItem = new ItemWithOptions<string>("Encryption", new string[]{"None","WPA/2"}, OnEncryptionOptionChanged, FirmwareSettings.WiFiSettings.Encryption ? 1 : 0);
			connectItem = new TurnWiFiOnOffCheckBox ();
			var childList = new List<IChildItem> ();
			childList.Add (ssidItem);
			childList.Add (passwordItem);
			childList.Add (encryptionItem);
			childList.Add (connectItem);
			return childList;
		}

		private void OnSsidChanged(string ssidName)
		{
			FirmwareSettings.WiFiSettings.SSID = ssidName;
			FirmwareSettings.Save ();
		}

		private void OnPasswordChanged(string password)
		{
			FirmwareSettings.WiFiSettings.Password = password;
			FirmwareSettings.Save ();
		}

		private void OnEncryptionOptionChanged(string option)
		{
			FirmwareSettings.WiFiSettings.Encryption = option != "None";
			FirmwareSettings.Save ();
		}

	}

	internal class TurnWiFiOnOffCheckBox : ItemWithCheckBoxStep
	{
		private const int ConnectTimeout = 40000;
		public TurnWiFiOnOffCheckBox(): base("Connected", WiFiDevice.IsLinkUp (), "WiFi", new CheckBoxStep(new StepContainer(OnTurnWiFiOn, "Connecting" ,"Failed to connect" ), new StepContainer(OnTurnWiFiOff, "Disconnecting", "Error disconnecting")), null)
		{
			base.OnCheckedChanged = CheckedChanged;	
		}

		private static bool OnTurnWiFiOn()
		{
			return WiFiDevice.TurnOn(FirmwareSettings.WiFiSettings.SSID, FirmwareSettings.WiFiSettings.Password, FirmwareSettings.WiFiSettings.Encryption, ConnectTimeout);
		}

		private static bool OnTurnWiFiOff()
		{
			WiFiDevice.TurnOff();
			return true;
		}

		private void CheckedChanged(bool isChecked)
		{
			if (isChecked) 
			{
				ConnectAtStartUpDialog dialog = new ConnectAtStartUpDialog ();
				dialog.SetFocus (this);
			}
		}
	}

	internal class ConnectAtStartUpDialog : ItemWithDialog<QuestionDialog>
	{
		public ConnectAtStartUpDialog(): base (new QuestionDialog("Do you want to connect at start-up?", "Settings"))
		{
		}

		public override void OnExit (QuestionDialog dialog)
		{
			if (dialog.IsPositiveSelected) 
			{
				FirmwareSettings.GeneralSettings.ConnectToWiFiAtStartUp = true;
				FirmwareSettings.GeneralSettings.CheckForSwUpdatesAtStartUp = true;
				FirmwareSettings.Save();
			}
		}
	}
}

