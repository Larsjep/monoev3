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
		private FirmwareSettings settings = new FirmwareSettings();
		private LoadSettingsDialog loadSettingsDialog;
		private object writeLock = new object();
		public ItemWithSettings(): base("Settings", Font.MediumFont, true)
		{
			loadSettingsDialog = new LoadSettingsDialog (OnSettingsLoaded);
		}

		public override void OnEnterPressed ()
		{
			Console.WriteLine ("Enter pressed item with settings. Show: " + show);
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
			autoConnect = new ItemWithCheckBox ("WiFi auto connect", settings.GeneralSettings.ConnectToWiFiAtStartUp);
			autoConnect.OnCheckedChanged += OnAutoConnectChanged;
			autoCheck = new ItemWithCheckBox("Update check",settings.GeneralSettings.CheckForSwUpdatesAtStartUp);
			autoCheck.OnCheckedChanged += OnAutoCheckChanged;
			var childList = new List<IChildItem> ();
			childList.Add (autoConnect);
			return childList;
		}

		private void OnSettingsLoaded(FirmwareSettings newSettings)
		{
			settings = newSettings;
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
					settings.GeneralSettings.ConnectToWiFiAtStartUp = newValue;
					settings.Save ();
				}
			}).Start ();
			
		}


		private void OnAutoCheckChanged(bool newValue)
		{
			new Thread (delegate() 
			{
				lock(writeLock){
					settings.GeneralSettings.CheckForSwUpdatesAtStartUp = newValue;
					settings.Save ();
				}
			}).Start ();
		}
	}

	internal class LoadSettingsDialog : ItemWithDialog<ProgressDialog>
	{

		private static FirmwareSettings Settings = new FirmwareSettings();
		private Action<FirmwareSettings> OnNewSettings;
		public LoadSettingsDialog(Action<FirmwareSettings> OnNewSettings): base(new ProgressDialog("Settings", new StepContainer( 
			()=>{
				Settings = Settings.Load();
				return Settings!=null;
			}, 
			"Loading", 
			"Failed to load settings")))
		{
			this.OnNewSettings = OnNewSettings;
		}

		public override void OnExit (ProgressDialog dialog)
		{
			OnNewSettings (Settings);
			if (!dialog.Ok) 
			{
				IChildItem parrentAsChild = ((IChildItem)Parent);
				parrentAsChild.Parent.RemoveFocus (parrentAsChild); //prevent from accessing the settings
			} 
			else 
			{
				Parent.RemoveFocus (this);
			}
		}
	}
}

