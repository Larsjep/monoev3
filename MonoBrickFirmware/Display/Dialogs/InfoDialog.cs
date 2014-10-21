using System;
using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class InfoDialog: Dialog{
		private string message;
		private bool waitForOk;
		private const string okString = "OK";
		public InfoDialog(string message, bool waitForOk, string title = "Information"):base(Font.MediumFont,title){
			this.message = message;
			this.waitForOk = waitForOk;
		}
		
		public void UpdateMessage(string message){
			this.message  = message;
			Draw();
		}
		
		/// <summary>
		/// Show the dialog - this does not block only draws the dialog if wait for OK is disabled
		/// </summary>
		public override bool Show (CancellationToken token)
		{
			if (waitForOk) {
				base.Show (token);
			} 
			else {
				OnShow();
				Draw ();
				OnExit();
				//Don't listen for button events
			}
			return true;
		}
		
		protected override bool OnEnterAction ()
		{
			return true;//exit
		}
		
		
		protected override void OnDrawContent ()
		{
			WriteTextOnDialog(message);
			if (waitForOk) {
				DrawCenterButton(okString, false);
			}
		}
	}
}

