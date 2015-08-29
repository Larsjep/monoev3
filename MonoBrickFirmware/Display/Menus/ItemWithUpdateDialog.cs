using System;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.FirmwareUpdate;
using System.Collections.Generic;
using MonoBrickFirmware.Device;
using System.Threading;
using MonoBrickFirmware.UserInput;


namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithUpdateDialog : ChildItemWithParent
	{
		private bool newImage = false;
		private bool newFirmwareApp = false;
		private bool newAddin = false;
		private ItemWithDialog<ProgressDialog> checkDialog = null;
		private ItemWithDialog<InfoDialog> infoDialog = null;
		private ItemWithDialog<QuestionDialog> questionDialog = null;
		private ItemWithDialog<StepDialog> updateDialog = null;
		public ItemWithUpdateDialog () : base("Check for update")
		{
			checkDialog = new ItemWithDialog<ProgressDialog>(new ProgressDialog("Updates", new StepContainer(CheckForUpdate,"Checking server", "Failed to check for Updates")));
			questionDialog = new ItemWithDialog<QuestionDialog>( new QuestionDialog ("New firmware available. Update?", "New Fiwmware"));
			updateDialog = new ItemWithDialog<StepDialog> 
			(
				new StepDialog ("Updating", 
					new List<IStep> {
						new StepContainer (UpdateHelper.DownloadFirmware, "Downloading...", "Failed to download files"),
						new StepContainer (UpdateHelper.UpdateBootFile, "Updating system", "Failed to update boot file")
					}
				)
			);
		}

		public override void OnEnterPressed ()
		{
			checkDialog.SetFocus (this,OnCheckCompleted);
		}

		private void OnQuestionCompleted(QuestionDialog dialog)
		{
			if (dialog.IsPositiveSelected) 
			{
				updateDialog.SetFocus (this, OnUpdateCompleted);
			} 	
		}

		private void OnUpdateCompleted(StepDialog dialog)
		{
			if (dialog.ExecutedOk)
			{
				Parent.SuspendButtonEvents ();
				Lcd.Clear ();
				Lcd.WriteText (Font.MediumFont, new Point (0, 0), "Shutting down...", true);
				Lcd.Update ();
				Buttons.LedPattern (2);
				Brick.TurnOff ();
				var whyAreYouHereDialog = new InfoDialog ("Cut the power", "Reboot failed");
				whyAreYouHereDialog.Show ();
				Lcd.Clear ();
				new ManualResetEvent (false).WaitOne ();
			} 
		}

		private void OnCheckCompleted(ProgressDialog dialog)
		{
			if (!checkDialog.Dialog.Ok) 
			{
				Parent.RemoveFocus (this);
				return;
			}

			if (newImage) 
			{
				infoDialog = new ItemWithDialog<InfoDialog>( new InfoDialog("New image available. Download it from www.monobrick.dk or ftp://soborg.net"));
				infoDialog.SetFocus (this);
			} 
			else {
				if (newFirmwareApp)
				{
					questionDialog.SetFocus (this, OnQuestionCompleted);
				} 
				else 
				{
					if (newAddin) 
					{
						infoDialog = new ItemWithDialog<InfoDialog>( new InfoDialog("New Xamarin Add-in. Download it from www.monobrick.dk or ftp://soborg.net"));
						infoDialog.SetFocus (this);
					} 
					else 
					{
						infoDialog = new ItemWithDialog<InfoDialog>( new InfoDialog("No updates available"));
						infoDialog.SetFocus (this);
					} 
				}
			}		
		}

		private bool CheckForUpdate()
		{
			newImage = false;
			newFirmwareApp = false;
			newAddin = false;
			VersionInfo versionInfo = null;
			try 
			{
				versionInfo = UpdateHelper.AvailableVersions();
				var currentVersion = UpdateHelper.InstalledVersion();
				newImage = versionInfo.Image != currentVersion.Image;
				newFirmwareApp = versionInfo.Firmware != currentVersion.Firmware;
				if (currentVersion.AddIn != null)
					newAddin = versionInfo.AddIn != currentVersion.AddIn;
			} 
			catch 
			{
				return false;
			}
			return true;
		}

	}
}

