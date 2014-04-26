using System;
using MonoBrickFirmware.UserInput;
using System.Collections.Generic;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class ProgressDialog : Dialog
	{
		private IStep step;
		private int progressLine;
		
		public ProgressDialog (Lcd lcd, Buttons btns, string title,IStep step): base(Font.MediumFont, lcd, btns, title)
		{
			this.step = step;
			progressLine = 1;
		}
		
		public override bool Show ()
		{
			bool ok = true;
			string endText;
			OnShow ();
			Draw ();
			StartProgressAnimation (progressLine);
			try {
				if (step.Execute ()) {
					endText = step.OkText;
				} else {
					ok = false;
					endText = step.ErrorText;
				}
			} 
			catch (Exception e) 
			{
				ok = false;
				endText =  "Exception executing " + step.StepText;
				Console.WriteLine("Exception " + e.Message);
				Console.WriteLine(e.StackTrace);
			}
			StopProgressAnimation ();
			if ((step.ShowOkText && ok) || !ok) 
			{
				ClearContent ();
				WriteTextOnDialog (endText);
				DrawCenterButton ("Ok", false);
				lcd.Update ();
				btns.GetKeypress ();//Wait for any key
			}
			OnExit();
			return ok;
		}
		
		protected override void OnDrawContent ()
		{
			WriteTextOnLine(step.StepText, 0);
			WriteTextOnLine("Please wait...", 2);
		}
	}
}

