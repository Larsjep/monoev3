using System;
using MonoBrickFirmware.UserInput;
using System.Collections.Generic;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class StepDialog : Dialog 
	{
		private List<IDialogStep> steps;
		private int stepIndex = 0;
		private int infoLineIndex;
		private int stepLineIndex;
		IDialogStep errorStep = null;
		
		public StepDialog (Lcd lcd, Buttons btns, string title,List<IDialogStep> steps): base(Font.MediumFont, lcd, btns, title)
		{
			this.steps = steps;
			infoLineIndex = numberOfLines/2;
			stepLineIndex = infoLineIndex+1;
		}
		
		protected override bool OnEnterAction ()
		{
			return true;//exit
		}
		
		public override bool Show ()
		{
			bool ok = true;
			errorStep = null;
			OnShow();
			for (stepIndex = 0; stepIndex < steps.Count; stepIndex++) {
				Draw ();
				if (!steps [stepIndex].Execute ()) 
				{
					ClearContent();
					WriteTextOnDialog(steps[stepIndex].ErrorText);
					DrawCenterButton("Ok",true);
					lcd.Update();
					btns.GetKeypress();//Wait for any key
					errorStep = steps [stepIndex];
					ok = false;
					break;
				}
			}
			OnExit();
			return ok;
		}
		
		public IDialogStep ErrorStep{ get {return errorStep;}}
		
		protected override void OnDrawContent ()
		{
			WriteTextOnLine(steps[stepIndex].Text,infoLineIndex);
			WriteTextOnLine("Step " + (stepIndex +1).ToString() + " of " + steps.Count, stepLineIndex);
			WriteTextOnLine("Please wait...", stepLineIndex +1);
		}
	}
}

