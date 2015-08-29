using System;
using MonoBrickFirmware.Display;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithOptions<OptionType> : ChildItem
	{
		
		private string text;
		private OptionType[] options;
		private const int rightArrowOffset = 4;
		private const int arrowEdge = 4;
		private Action<OptionType> OnOptionChanged;

		public ItemWithOptions(string text, OptionType[] options, Action<OptionType> OnOptionChanged, int startIdx = 0)
        {
			this.text = text;
			this.options = options;
			this.OptionIndex = startIdx;
			this.OnOptionChanged = OnOptionChanged;
		}
		public override void OnEnterPressed()
		{
			OnRightPressed();
		}
		
		public override void OnLeftPressed ()
		{
			OptionIndex = OptionIndex -1;
			if(OptionIndex < 0)
				OptionIndex = options.Length-1;
			OnOptionChanged(options[OptionIndex]);
		}

		public override void OnRightPressed(){
			OptionIndex = (OptionIndex+1)%options.Length;
			OnOptionChanged(options[OptionIndex]);

		}

		public override void OnDrawTitle (Font f, Rectangle r, bool color)
		{
			int arrowWidth = (int)f.maxWidth / 4;
			
			string valueAsString = " " + options[OptionIndex].ToString() + " ";
			Point p = f.TextSize (valueAsString);
			Rectangle numericRect = new Rectangle ( new Point( Lcd.Width - p.X, r.P1.Y),r.P2);
			Rectangle textRect = new Rectangle (new Point (r.P1.X, r.P1.Y), new Point (r.P2.X - (p.X), r.P2.Y));
			Rectangle leftArrowRect = new Rectangle(new Point(numericRect.P1.X, numericRect.P1.Y+arrowEdge), new Point(numericRect.P1.X+ arrowWidth, numericRect.P2.Y-arrowEdge));
			Rectangle rightArrowRect = new Rectangle( new Point(numericRect.P2.X-(arrowWidth + rightArrowOffset), numericRect.P1.Y+arrowEdge) , new Point(numericRect.P2.X-rightArrowOffset,numericRect.P2.Y-arrowEdge));
			
			Lcd.WriteTextBox (f, textRect, text, color, Lcd.Alignment.Left);
			Lcd.WriteTextBox (f, numericRect, valueAsString, color, Lcd.Alignment.Right);
			Lcd.DrawArrow(leftArrowRect, Lcd.ArrowOrientation.Left, color);
			Lcd.DrawArrow(rightArrowRect, Lcd.ArrowOrientation.Right, color);

		}

		public int OptionIndex{get;private set;}

        public OptionType GetSelection() {
            return options[OptionIndex];
        }

	}
}

