using System;
using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class QuestionDialog : InfoDialog
	{
		private string negativeText;
		private string positiveText;
		private int textSize = 0;

		public QuestionDialog (string question, string title, string positiveText="Yes", string negativeText="No", bool isPositiveSelected = true) : base (question, title)
		{
			this.negativeText = negativeText;
			this.positiveText = positiveText;
			this.IsPositiveSelected = isPositiveSelected;
			int positiveSize = font.TextSize (positiveText).X;
			int negativeSize = font.TextSize (negativeText).X;
			if (positiveSize > negativeSize) 
			{
				textSize = positiveSize;
			} 
			else 
			{
				textSize = negativeSize;
			}
		}
		
		internal override void OnLeftPressed ()
		{
			if(!IsPositiveSelected)
				IsPositiveSelected = true;
		}
		
		internal override void OnRightPressed ()
		{
			if(IsPositiveSelected)
				IsPositiveSelected = false;
		}
		
		protected override void OnDrawContent ()
		{
			DrawText ();
			DrawLeftButton(positiveText, !IsPositiveSelected, textSize);
			DrawRightButton(negativeText,IsPositiveSelected, textSize);
		}

		public override bool Show ()
		{
			base.Show ();
			return IsPositiveSelected;
		}
		
		public bool IsPositiveSelected{get; private set;}
		
	}
}

