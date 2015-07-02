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
		private Thread progress = null;
		private ManualResetEvent waitForOk = new ManualResetEvent (false);
		public ProgressDialog (string title,IStep step): base(Font.MediumFont, title)
		{
			this.step = step;
			progressLine = 1;
			CreateProcessThread ();
		}
		
		internal override void Draw ()
		{
			if (!progress.IsAlive) 
			{
				CreateProcessThread ();
				progress.Start ();
			} 
		}

		public bool Ok{ get; private set;}

		public override void Hide ()
		{
			base.Hide ();
			if (progress.IsAlive) 
			{
				progress.Join ();
			}
		}

		protected override void OnDrawContent ()
		{
			WriteTextOnLine(step.StepText, 0);
			WriteTextOnLine("Please wait...", 2);
		}

		/// <summary>
		/// Show menu. Returns true if step executed ok otherwise false
		/// </summary>
		public override bool Show ()
		{
			base.Show ();
			return Ok;
		}

		internal override void OnEnterPressed ()
		{
			waitForOk.Set ();	
		}

		private void CreateProcessThread()
		{
			progress = new Thread (delegate() 
			{
				Ok = true;
				string endText;
				StartProgressAnimation (progressLine);
				OnShow ();
				base.Draw();
				try {
					
					if (step.Execute ()) 
					{
						endText = step.OkText;
					} 
					else 
					{
						Ok = false;
						endText = step.ErrorText;
					}
				} 
				catch (Exception e) 
				{
					Ok = false;
					endText =  "Exception executing " + step.StepText;
					Console.WriteLine("Exception " + e.Message);
					Console.WriteLine(e.StackTrace);
				}
				StopProgressAnimation ();
				if ((step.ShowOkText && Ok) || !Ok) 
				{
					ClearContent ();
					WriteTextOnDialog (endText);
					DrawCenterButton ("Ok", false);
					waitForOk.Reset();
					Lcd.Instance.Update ();
					waitForOk.WaitOne();
				}
				OnExit();
			});
		
		}
	}
}

