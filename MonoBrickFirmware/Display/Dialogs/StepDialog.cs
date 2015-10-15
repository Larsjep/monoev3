using System;
using System.Threading;
using MonoBrickFirmware.UserInput;
using System.Collections.Generic;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class StepDialog : DialogWithProgessAnimation 
	{
		private List<IStep> steps;
		private int stepIndex = 0;
		private int infoLineIndex;
		private int stepLineIndex;
		private string allDoneText;
		private IStep errorStep = null;
		private int progressLine = 1;
		private CancellationTokenSource cancelSource;
		private Thread stepThread=null;
		private ManualResetEvent waitForOk = new ManualResetEvent (false);
		public StepDialog (string title,List<IStep> steps): this(title,steps,"")		
		{
			
		}
		
		public StepDialog (string title,List<IStep> steps, string allCompletedText): base(Font.MediumFont, title)
		{
			this.steps = steps;
			infoLineIndex = 0;
			progressLine = 1;
			stepLineIndex = 2;
			this.allDoneText = allCompletedText;
			CreateStepThread ();
		}

		internal override void Draw ()
		{
			if (!stepThread.IsAlive)
			{
				CreateStepThread ();
				stepThread.Start ();
			}						
		}

		public override void Hide ()
		{
			base.Hide ();
			if (cancelSource != null && stepThread.IsAlive) 
			{
				cancelSource.Cancel ();
				stepThread.Join ();
			}
		}

		internal override void OnEnterPressed ()
		{
			waitForOk.Set ();
		}
		
		public IStep ErrorStep{ get {return errorStep;}}
		
		protected override void OnDrawContent ()
		{
			WriteTextOnLine(steps[stepIndex].StepText,infoLineIndex);
			WriteTextOnLine("Step " + (stepIndex +1).ToString() + " of " + steps.Count, stepLineIndex);
		}

		/// <summary>
		/// Show menu. Returns true if the step executed without problem
		/// </summary>
		public override bool Show ()
		{
			base.Show ();
			return errorStep == null;
		}

		public bool ExecutedOk{get{return errorStep == null;}}


		private void CreateStepThread()
		{
			stepThread = new Thread(delegate() {
				cancelSource = new CancellationTokenSource ();
				var token = cancelSource.Token;
				bool ok = true;
				errorStep = null;
				base.Draw();
				StartProgressAnimation (progressLine);
				for (stepIndex = 0; stepIndex < steps.Count; stepIndex++) {
					base.Draw();;
					try {
						if(token.IsCancellationRequested)
						{
							StopProgressAnimation ();
							ClearContent ();
							ok = false;
							break;

						}
						if (!steps[stepIndex].Execute ()) {
							StopProgressAnimation ();
							ClearContent ();
							WriteTextOnDialog (steps [stepIndex].ErrorText);
							DrawCenterButton ("Ok", false);
							Lcd.Update ();
							Buttons.GetKeypress ();//Wait for any key
							errorStep = steps [stepIndex];
							ok = false;
							break;
						} 
						else if (steps[stepIndex].ShowOkText) 
						{
							StopProgressAnimation ();
							ClearContent ();
							WriteTextOnDialog (steps [stepIndex].OkText);
							DrawCenterButton ("Ok", false);
							waitForOk.Reset(); 
							Lcd.Update ();
							waitForOk.WaitOne();
							StartProgressAnimation (progressLine);
						}
					} 
					catch 
					{
						StopProgressAnimation ();
						ClearContent ();
						WriteTextOnDialog ("Exception excuting " + steps [stepIndex].StepText);
						DrawCenterButton ("Ok", false);
						waitForOk.Reset();
						Lcd.Update ();
						waitForOk.WaitOne();
						errorStep = steps [stepIndex];
						ok = false;
						break;
					}
				}
				stepIndex = 0;
				StopProgressAnimation ();
				if (allDoneText != "" && ok && !token.IsCancellationRequested) 
				{
					ClearContent ();
					WriteTextOnDialog (allDoneText);
					DrawCenterButton ("Ok", false);
					Lcd.Update ();
					Buttons.GetKeypress ();//Wait for any key*/
				}
				OnExit();
			});

		}
	}
}

