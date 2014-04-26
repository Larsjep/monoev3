using System;
using MonoBrickFirmware.UserInput;
using System.Collections.Generic;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class StepDialog : Dialog 
	{
		private List<IStep> steps;
		private int stepIndex = 0;
		private int infoLineIndex;
		private int stepLineIndex;
		private string allDoneText;
		private IStep errorStep = null;
		private int progressLine = 1;
		
		public StepDialog (Lcd lcd, Buttons btns, string title,List<IStep> steps): this(lcd, btns,title,steps,"")		
		{
		
		}
		
		public StepDialog (Lcd lcd, Buttons btns, string title,List<IStep> steps, string allCompletedText): base(Font.MediumFont, lcd, btns, title)
		{
			this.steps = steps;
			infoLineIndex = 0;
			progressLine = 1;
			stepLineIndex = 2;
			this.allDoneText = allCompletedText;
		}
		
		
		
		protected override bool OnEnterAction ()
		{
			return true;//exit
		}
		
		public override bool Show ()
		{
			bool ok = true;
			errorStep = null;
			OnShow ();
			Draw ();
			StartProgressAnimation (progressLine);
			for (stepIndex = 0; stepIndex < steps.Count; stepIndex++) {
				Draw ();
				if (!steps [stepIndex].Execute ()) {
					StopProgressAnimation ();
					ClearContent ();
					WriteTextOnDialog (steps [stepIndex].ErrorText);
					DrawCenterButton ("Ok", false);
					lcd.Update ();
					btns.GetKeypress ();//Wait for any key
					errorStep = steps [stepIndex];
					ok = false;
					break;
				} 
				else {
					
					if (steps [stepIndex].ShowOkText) 
					{
						StopProgressAnimation ();
						ClearContent ();
						WriteTextOnDialog (steps [stepIndex].ErrorText);
						DrawCenterButton ("Ok", false);
						lcd.Update ();
						btns.GetKeypress ();//Wait for any key
						StartProgressAnimation(progressLine);
					}
				
				
				}
			}
			StopProgressAnimation ();
			if (allDoneText != "" && ok) {
				ClearContent();
				WriteTextOnDialog(allDoneText);
				DrawCenterButton("Ok",false);
				lcd.Update();
				btns.GetKeypress();//Wait for any key*/
			}
			OnExit();
			return ok;
		}
		
		public IStep ErrorStep{ get {return errorStep;}}
		
		protected override void OnDrawContent ()
		{
			WriteTextOnLine(steps[stepIndex].StepText,infoLineIndex);
			WriteTextOnLine("Step " + (stepIndex +1).ToString() + " of " + steps.Count, stepLineIndex);
		}
	}
}

