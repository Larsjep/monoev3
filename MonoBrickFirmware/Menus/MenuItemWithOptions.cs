using System;
using MonoBrickFirmware.Display;

namespace MonoBrickFirmware.Menus
{
	public class MenuItemWithOptions<OptionType> : IMenuItem
	{
		private string text;
		private Lcd lcd;
        private OptionType[] options;
		private const int rightArrowOffset = 4;
		private const int arrowEdge = 4;
        public MenuItemWithOptions(Lcd lcd, string text, OptionType[] options, int startIdx = 0)
        {
			this.text = text;
			this.lcd = lcd;
			this.options = options;
			this.OptionIndex = startIdx;
		}
		public bool EnterAction()
		{
			return false;
		}
		
		public bool LeftAction ()
		{
			OptionIndex = OptionIndex -1;
			if(OptionIndex < 0)
				OptionIndex = options.Length-1;
			return false;
		}
		public bool RightAction(){
			OptionIndex = (OptionIndex+1)%options.Length;
			return false;
		}
		public void Draw (Font f, Rectangle r, bool color)
		{
			int arrowWidth = (int)f.maxWidth / 4;
			
			string valueAsString = " " + options[OptionIndex].ToString() + " ";
			Point p = f.TextSize (valueAsString);
			Rectangle numericRect = new Rectangle ( new Point( Lcd.Width - p.X, r.P1.Y),r.P2);
			Rectangle textRect = new Rectangle (new Point (r.P1.X, r.P1.Y), new Point (r.P2.X - (p.X), r.P2.Y));
			Rectangle leftArrowRect = new Rectangle(new Point(numericRect.P1.X, numericRect.P1.Y+arrowEdge), new Point(numericRect.P1.X+ arrowWidth, numericRect.P2.Y-arrowEdge));
			Rectangle rightArrowRect = new Rectangle( new Point(numericRect.P2.X-(arrowWidth + rightArrowOffset), numericRect.P1.Y+arrowEdge) , new Point(numericRect.P2.X-rightArrowOffset,numericRect.P2.Y-arrowEdge));
			
			lcd.WriteTextBox (f, textRect, text, color, Lcd.Alignment.Left);
			lcd.WriteTextBox (f, numericRect, valueAsString, color, Lcd.Alignment.Right);
			lcd.DrawArrow(leftArrowRect, Lcd.ArrowOrientation.Left, color);
			lcd.DrawArrow(rightArrowRect, Lcd.ArrowOrientation.Right, color);
		}
		public int OptionIndex{get;private set;}

        OptionType GetSelection() {
            return options[OptionIndex];
        }

	}
}

