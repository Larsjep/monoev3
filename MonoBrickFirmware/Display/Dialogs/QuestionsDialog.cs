using System;
using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class QuestionDialog : Dialog{
		private string negativeText;
		private string positiveText;
		private int textSize = 0;
		private string question;
		
		public QuestionDialog (string question, string title, string positiveText="Yes", string negativeText="No", bool isPositiveSelected = true) : base (Font.MediumFont, title)
		{
			this.negativeText = negativeText;
			this.positiveText = positiveText;
			this.question = question;
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
		
		internal override void OnEnterPressed ()
		{
			OnExit();
		}
		
		protected override void OnDrawContent ()
		{
			WriteTextOnDialog(question);
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

