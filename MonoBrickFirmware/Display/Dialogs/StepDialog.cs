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
		private MonoBrickFirmware.Display.Animation.ProgressAnimation progress;
		IStep errorStep = null;
		
		public StepDialog (Lcd lcd, Buttons btns, string title,List<IStep> steps): base(Font.MediumFont, lcd, btns, title)
		{
			this.steps = steps;
			infoLineIndex = 0;
			Rectangle progressRect = GetLineRectangle(1);
			progressRect = new Rectangle( progressRect.P1 + new Point(10,8), progressRect.P2 + new Point(-10,-8));
			progress = new MonoBrickFirmware.Display.Animation.ProgressAnimation(lcd, progressRect);
			stepLineIndex = 2;
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
			Draw ();
			progress.Start();
			for (stepIndex = 0; stepIndex < steps.Count; stepIndex++) {
				Draw ();
				if (!steps [stepIndex].Execute ()) 
				{
					progress.Stop();
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
			progress.Stop();
			ClearContent();
			WriteTextOnDialog("Done!");
			DrawCenterButton("Ok",false);
			lcd.Update();
			btns.GetKeypress();//Wait for any key
			OnExit();
			return ok;
		}
		
		public IStep ErrorStep{ get {return errorStep;}}
		
		protected override void OnDrawContent ()
		{
			WriteTextOnLine(steps[stepIndex].Text,infoLineIndex);
			WriteTextOnLine("Step " + (stepIndex +1).ToString() + " of " + steps.Count, stepLineIndex);
		}
	}
}

