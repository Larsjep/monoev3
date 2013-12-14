using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Dialogs
{
	public class QuestionDialog : Dialog{
		private string negativeText;
		private string positiveText;
		private string question;
		private const int boxEdge = 2;
		private const int boxMiddleOffset = 8;
		private const int textOffset = 2;
		public QuestionDialog (Font f, Lcd lcd, Buttons btns, string question, string title, string positiveText="Yes", string negativeText="No", bool isPositiveSelected = true) : base (f, lcd, btns, title)
		{
			this.negativeText = negativeText;
			this.positiveText = positiveText;
			this.question = question;
			this.IsPositiveSelected = isPositiveSelected;
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
			int textSize = 0;
			if (font.TextSize (positiveText).X > font.TextSize (negativeText).X) 
			{
				textSize = font.TextSize(positiveText).X;
			} 
			else 
			{
				textSize = font.TextSize(negativeText).X;
			}
			textSize+=textOffset;
			
			Point positive1 = bottomLineCenter + new Point(-boxMiddleOffset - (int)textSize,(int)-font.maxHeight/2);
			Point positive2 = bottomLineCenter + new Point(-boxMiddleOffset,(int)font.maxHeight/2);
			Point positiveOuter1 = positive1 + new Point(-boxEdge,-boxEdge);
			Point positiveOuter2 = positive2 + new Point(boxEdge,boxEdge);
			
			
			Point negative1 = bottomLineCenter + new Point(boxMiddleOffset,(int)-font.maxHeight/2);
			Point negative2 = bottomLineCenter + new Point(boxMiddleOffset + (int)textSize,(int)font.maxHeight/2);
			Point negativeOuter1 = negative1 + new Point(-boxEdge,-boxEdge);
			Point negativeOuter2 = negative2 + new Point(boxEdge,boxEdge);
			
			
			Rectangle positiveRect = new Rectangle(positive1, positive2);
			Rectangle negativeRect = new Rectangle(negative1, negative2);
			Rectangle positiveOuterRect = new Rectangle(positiveOuter1, positiveOuter2);
			Rectangle negativeOuterRect = new Rectangle(negativeOuter1, negativeOuter2);
			
			lcd.DrawBox(positiveOuterRect,true);
			lcd.DrawBox(negativeOuterRect, true);
			
			lcd.WriteTextBox(font, positiveRect, positiveText, !IsPositiveSelected, Lcd.Alignment.Center);
			lcd.WriteTextBox(font, negativeRect, negativeText, IsPositiveSelected, Lcd.Alignment.Center);
		}
		
		public bool IsPositiveSelected{get; private set;}
	
	}
}

