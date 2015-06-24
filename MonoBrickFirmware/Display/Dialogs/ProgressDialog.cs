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
			else 
			{
				progress.Abort ();
				CreateProcessThread ();
				progress.Start ();
			}
		}

		public bool Ok{ get; private set;}
		
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

		private void CreateProcessThread()
		{
			progress = new Thread (delegate() 
			{
				Ok = true;
				string endText;
				OnShow ();
				base.Draw();
				StartProgressAnimation (progressLine);
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
					Lcd.Instance.Update ();
					Buttons.Instance.GetKeypress ();//Wait for any key
				}
				Console.WriteLine ("Progress is done");
				OnExit();
			});
		
		}
	}
}

