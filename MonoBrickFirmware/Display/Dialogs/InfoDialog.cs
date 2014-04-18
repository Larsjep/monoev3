using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class InfoDialog: Dialog{
		private string message;
		private bool waitForOk;
		private const string okString = "OK";
		public InfoDialog(Font f, Lcd lcd, Buttons btns, string message, bool waitForOk, string title = "Information"):base(f,lcd,btns,title){
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
		public override bool Show ()
		{
			if (waitForOk) {
				base.Show ();
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

