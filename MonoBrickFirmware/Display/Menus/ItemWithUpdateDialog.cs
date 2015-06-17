using System;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.FirmwareUpdate;
using System.Collections.Generic;
using MonoBrickFirmware.Native;
using System.Threading;


namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithUpdateDialog : ItemWithDialog<ProgressDialog>, IParentItem
	{
		private static bool newImage = false;
		private static bool newFirmwareApp = false;
		private static bool newAddin = false;

		public ItemWithUpdateDialog () : base( new ProgressDialog ("Updates", new StepContainer(CheckForUpdate,"Checking server", "Failed to check for Updates")))
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
					UpdateQuestionDialog updateQuestion = new UpdateQuestionDialog ();
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
				versionInfo = VersionHelper.AvailableVersions ();
				var currentVersion = VersionHelper.CurrentVersions();
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

		public void SuspendEvents (IChildItem item)
		{
			Parent.SuspendEvents (item);
		}

		public void ResumeEvents (IChildItem item)
		{
			Parent.SuspendEvents (item);
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
				Parent.SuspendEvents (this);
				for (int seconds = 10; seconds > 0; seconds--) 
				{
					var rebootDialog = new InfoDialog ("Update completed. Rebooting in  " + seconds, false);
					rebootDialog.Show ();
					Thread.Sleep (1000);
				}
				ProcessHelper.RunAndWaitForProcess ("/sbin/shutdown", "-h now");
				Thread.Sleep (120000);
				var whyAreYouHereDialog = new InfoDialog ("Cut the power", false, "Reboot failed");
				whyAreYouHereDialog.Show ();
				new ManualResetEvent (false).WaitOne ();
			}
			Parent.RemoveFocus (this);
		}

	}

	internal class UpdateQuestionDialog : ItemWithDialog<QuestionDialog>
	{
		public UpdateQuestionDialog() : base(new QuestionDialog ("New firmware available. Update?", "New Fiwmware"))
		{
		
		}

		public override void OnExit (QuestionDialog dialog)
		{
			if (dialog.IsPositiveSelected) 
			{
			
			} 

			else 
			{
				Parent.RemoveFocus (this);
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

