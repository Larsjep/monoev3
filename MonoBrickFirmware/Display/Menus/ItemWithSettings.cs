using System;
using System.Collections.Generic;
using MonoBrickFirmware.Settings;
using MonoBrickFirmware.Display.Dialogs;
using System.Threading;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithSettings : ItemList
	{
		private ItemWithCheckBox autoConnect;
		private ItemWithCheckBox autoCheck;
		private object writeLock = new object();
		public ItemWithSettings(): base("Settings", Font.MediumFont, true)
		{
		}

		protected override List<IChildItem> OnCreateChildList ()
		{
			autoConnect = new ItemWithCheckBox ("WiFi auto connect", FirmwareSettings.GeneralSettings.ConnectToWiFiAtStartUp);
			autoConnect.OnCheckedChanged += OnAutoConnectChanged;
			autoCheck = new ItemWithCheckBox("Update check",FirmwareSettings.GeneralSettings.CheckForSwUpdatesAtStartUp);
			autoCheck.OnCheckedChanged += OnAutoCheckChanged;
			var childList = new List<IChildItem> ();
			childList.Add (autoConnect);
			childList.Add (autoCheck);
			return childList;
		}

		private void OnAutoConnectChanged(bool newValue)
		{
			if (!newValue && autoCheck.Checked) 
			{
				autoCheck.OnEnterPressed ();
			}
			new Thread (delegate() 
			{
				lock(writeLock){
					FirmwareSettings.GeneralSettings.ConnectToWiFiAtStartUp = newValue;
					FirmwareSettings.Save ();
				}
			}).Start ();
			
		}


		private void OnAutoCheckChanged(bool newValue)
		{
			new Thread (delegate() 
			{
				lock(writeLock){
					FirmwareSettings.GeneralSettings.CheckForSwUpdatesAtStartUp = newValue;
					FirmwareSettings.Save ();
				}
			}).Start ();
		}
	}
}

