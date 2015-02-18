using System;
using System.Threading;
using MonoBrickFirmware.UserInput;
using System.Collections.Generic;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class ProgressDialog : DialogWithProgessAnimation
	{
		private IStep step;
		private int progressLine;
		
		public ProgressDialog (string title,IStep step): base(Font.MediumFont, title)
		{
			this.step = step;
			progressLine = 1;
		}
		
		public override bool Show (CancellationToken token)
		{
			bool ok = true;
			string endText;
			OnShow ();
			Draw ();
			StartProgressAnimation (progressLine);
			try {
				if (step.Execute ()) 
				{
					endText = step.OkText;
				} 
				else 
				{
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
				Lcd.Instance.Update ();
				Buttons.Instance.GetKeypress ();//Wait for any key
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

