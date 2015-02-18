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
		
		public override bool Show (CancellationToken token)
		{
			base.Show (token);
			return IsPositiveSelected;
		}
		
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
		
		protected override bool OnLeftAction ()
		{
			if(!IsPositiveSelected)
				IsPositiveSelected = true;
			return false;
		}
		
		protected override bool OnRightAction ()
		{
			if(IsPositiveSelected)
				IsPositiveSelected = false;
			return false;
		}
		
		protected override bool OnEnterAction ()
		{
			return true;
		}
		
		protected override void OnDrawContent ()
		{
			WriteTextOnDialog(question);
			DrawLeftButton(positiveText, !IsPositiveSelected, textSize);
			DrawRightButton(negativeText,IsPositiveSelected, textSize);
		}
		
		public bool IsPositiveSelected{get; private set;}
		
	}
}

