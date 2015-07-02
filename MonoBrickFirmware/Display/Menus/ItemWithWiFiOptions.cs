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
		private FirmwareSettings settings = new FirmwareSettings();
		private LoadSettingsDialog loadSettingsDialog;
		public ItemWiFiOptions(): base("WiFi", Font.MediumFont, true)
		{
			loadSettingsDialog = new LoadSettingsDialog (OnSettingsLoaded);
		}

		public override void OnEnterPressed ()
		{
			if (!show) 
			{
				loadSettingsDialog.SetFocus (this);	
			} 
			else 
			{
				base.OnEnterPressed ();
			}
		}

		protected override List<IChildItem> OnCreateChildList ()
		{
			ssidItem = new ItemWithCharacterInput("SSID", "Enter SSID", settings.WiFiSettings.SSID);
			ssidItem.OnDialogExit += OnSsidChanged;
			passwordItem = new ItemWithCharacterInput("Password", "Password", settings.WiFiSettings.Password, true);
			passwordItem.OnDialogExit += OnPasswordChanged;
			encryptionItem = new ItemWithOptions<string>("Encryption", new string[]{"None","WPA/2"}, settings.WiFiSettings.Encryption ? 1 : 0);
			encryptionItem.OnOptionChanged += OnEncryptionOptionChanged;
			connectItem = new TurnWiFiOnOffCheckBox (settings);
			var childList = new List<IChildItem> ();
			childList.Add (ssidItem);
			childList.Add (passwordItem);
			childList.Add (encryptionItem);
			childList.Add (connectItem);
			return childList;
		}

		private void OnSettingsLoaded(FirmwareSettings newSettings)
		{
			settings = newSettings;
		}

		private void OnSsidChanged(string ssidName)
		{
			settings.WiFiSettings.SSID = ssidName;
			settings.Save ();
		}

		private void OnPasswordChanged(string password)
		{
			settings.WiFiSettings.Password = password;
			settings.Save ();
		}

		private void OnEncryptionOptionChanged(string option)
		{
			settings.WiFiSettings.Encryption = option != "None";
			settings.Save ();
		}

	}

	internal class TurnWiFiOnOffCheckBox : ItemWithCheckBoxStep
	{
		private const int ConnectTimeout = 60000;
		private static FirmwareSettings settings = new FirmwareSettings();
		public TurnWiFiOnOffCheckBox(FirmwareSettings firmwareSettings): base("Connected", WiFiDevice.IsLinkUp (), "WiFi", new CheckBoxStep(new StepContainer(OnTurnWiFiOn, "Connecting" ,"Failed to connect" ), new StepContainer(OnTurnWiFiOff, "Disconnecting", "Error disconnecting")))
		{
			settings = firmwareSettings;
			this.OnCheckedChanged += CheckedChanged;
		}

		private static bool OnTurnWiFiOn()
		{
			if (!WiFiDevice.WriteWpaSupplicantConfiguration (settings.WiFiSettings.SSID, settings.WiFiSettings.Password, settings.WiFiSettings.Encryption))
				return false;
			return WiFiDevice.TurnOn(ConnectTimeout);
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
				ConnectAtStartUpDialog dialog = new ConnectAtStartUpDialog (settings);
				dialog.SetFocus (this);
			}
		}
	}

	internal class ConnectAtStartUpDialog : ItemWithDialog<QuestionDialog>
	{
		private FirmwareSettings settings;
		public ConnectAtStartUpDialog(FirmwareSettings settings): base (new QuestionDialog("Do you want to connect at start-up?", "Settings"))
		{
			this.settings = settings;
		}

		public override void OnExit (QuestionDialog dialog)
		{
			if (dialog.IsPositiveSelected) 
			{
				settings.GeneralSettings.ConnectToWiFiAtStartUp = true;
				settings.GeneralSettings.CheckForSwUpdatesAtStartUp = true;
				settings.Save();
			}
		}
	}
}

