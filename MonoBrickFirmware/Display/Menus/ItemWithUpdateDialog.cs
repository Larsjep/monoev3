using System;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.FirmwareUpdate;
using System.Collections.Generic;
using MonoBrickFirmware.Device;
using System.Threading;
using MonoBrickFirmware.UserInput;


namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithUpdateDialog : ItemWithDialog<ProgressDialog>, IParentItem
	{
		private static bool newImage = false;
		private static bool newFirmwareApp = false;
		private static bool newAddin = false;

		public ItemWithUpdateDialog () : base( new ProgressDialog ("Updates", new StepContainer(CheckForUpdate,"Checking server", "Failed to check for Updates")), "Check for update")
		{
				
		}

		public override void OnExit (ProgressDialog dialog)
		{
			if (!dialog.Ok) 
			{
				Parent.RemoveFocus (this);
				return;
			}

			if (newImage) 
			{
				var visitDialog = new ItemWithInfoDialog ("New image available. Download it at monobrick.dk");
				visitDialog.SetFocus (this);
			} 
			else {
				if (newFirmwareApp)
				{
					UpdateQuestionDialog updateQuestion = new UpdateQuestionDialog (this);
					updateQuestion.SetFocus (this);
				} 
				else 
				{
					if (newAddin) 
					{
						var visitDialog = new ItemWithInfoDialog ("New Xamarin Add-in. Download it at monobrick.dk");
						visitDialog.SetFocus (this);
					} 
					else 
					{
						var noUpdateDialog = new ItemWithInfoDialog ("No updates available");
						noUpdateDialog.SetFocus (this);
					} 
				}
			}	
		}

		private static bool CheckForUpdate()
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

		public void SetFocus (IChildItem item)
		{
			Parent.SetFocus (item);
		}

		public void RemoveFocus (IChildItem item)
		{
			Parent.RemoveFocus (item);
		}

		public void SuspendButtonEvents ()
		{
			Parent.SuspendButtonEvents ();
		}

		public void ResumeButtonEvents ()
		{
			Parent.SuspendButtonEvents ();
		}
	}

	internal class UpdateProgressDialog : ItemWithDialog<StepDialog>
	{
		public UpdateProgressDialog() : base(new StepDialog ("Updating", 
			new List<IStep>
			{
				new StepContainer (UpdateHelper.DownloadFirmware, "Downloading...", "Failed to download files"),
				new StepContainer (UpdateHelper.UpdateBootFile, "Updating system", "Failed to update boot file")
			}
		))
		{
		
		}

		public override void OnExit (StepDialog dialog)
		{
			if (dialog.ExecutedOk) {
				Parent.SuspendButtonEvents ();
				Lcd.Clear();
				Lcd.WriteText(Font.MediumFont, new Point(0,0), "Shutting down...", true);
				Lcd.Update();
				Buttons.LedPattern(2);
				Brick.TurnOff ();
				var whyAreYouHereDialog = new InfoDialog ("Cut the power", false, "Reboot failed");
				whyAreYouHereDialog.Show ();
				new ManualResetEvent (false).WaitOne ();
			}
			Parent.RemoveFocus (this);
		}

	}

	internal class UpdateQuestionDialog : ItemWithDialog<QuestionDialog>
	{
		private IParentItem parent;
		private UpdateProgressDialog updateDialog = new UpdateProgressDialog();
		public UpdateQuestionDialog(IParentItem parent) : base(new QuestionDialog ("New firmware available. Update?", "New Fiwmware"))
		{
			this.parent = parent;	
		}

		public override void OnExit (QuestionDialog dialog)
		{
			if (dialog.IsPositiveSelected) 
			{
				updateDialog.SetFocus (parent);
			} 
		}

	}


	internal class ItemWithInfoDialog : ItemWithDialog<InfoDialog>
	{
		public ItemWithInfoDialog(string text) : base(new InfoDialog (text, true))
		{
		
		}

		public override void OnExit (InfoDialog dialog)
		{
			Parent.RemoveFocus (this);		
		}


	}
}

