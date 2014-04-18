using System;
using MonoBrickFirmware.UserInput;
using System.Collections.Generic;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class ProgressDialog : Dialog
	{
		private IStep step;
		private bool showError;
		public ProgressDialog (Lcd lcd, Buttons btns, string title,IStep step, bool showError): base(Font.MediumFont, lcd, btns, title)
		{
			this.step = step;
			this.showError = showError;
			CreateProgessAnimation(1);
		}
		
		public override bool Show ()
		{
			bool ok = true;
			OnShow ();
			Draw ();
			StartProgressAnimation ();
			if (!step.Execute ()) {
				if (showError) {
					StopProgressAnimation ();
					ClearContent ();
					WriteTextOnDialog (step.ErrorText);
					DrawCenterButton ("Ok", false);
					lcd.Update ();
					btns.GetKeypress ();//Wait for any key
				}
				ok = false;
			}
			StopProgressAnimation();
			OnExit();
			return ok;
		}
		
		protected override void OnDrawContent ()
		{
			WriteTextOnLine(step.Text, 0);
			WriteTextOnLine("Please wait...", 2);
		}
	}
}

