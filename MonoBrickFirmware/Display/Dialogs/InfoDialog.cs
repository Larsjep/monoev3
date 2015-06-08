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
		public InfoDialog(string message, bool waitForOk, string title = "Information"):base(Font.MediumFont,title)
		{
			this.message = message;
			this.waitForOk = waitForOk;
		}
		
		public void UpdateMessage(string message){
			this.message  = message;
			Draw();
		}
		
		/// <summary>
		/// Draw the dialog
		/// </summary>
		internal override void Draw ()
		{
			if (waitForOk) 
			{
				base.Draw();
			} 
			else 
			{
				OnShow();
				Draw ();
				OnExit();
			}
		}
		
		internal override void OnEnterPressed ()
		{
			OnExit();
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

