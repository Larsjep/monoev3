using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class SelectDialog<SelectionType> : Dialog { 
        private SelectionType[] options;
        private int scrollPos;
        int cursorPos;
		bool allowEsc;
		int arrowHeight = 5;
		int arrowWidth = 10;
		Rectangle arrowRect;
		
		public SelectDialog (SelectionType[] options, string title, bool allowEsc) : base (Font.MediumFont, title, 170,90+(int)Font.MediumFont.maxHeight/2,(int)Font.MediumFont.maxHeight/4)
		{
			this.options = options;
			cursorPos = 0;
			scrollPos = 0;
			this.allowEsc = allowEsc;
			EscPressed = false;
			int yEdge = (Lcd.Height - outherWindow.P2.Y);
			int dialogEdge = outherWindow.P2.Y - innerWindow.P2.Y;
			arrowRect = new Rectangle (new Point (Lcd.Width / 2 - arrowWidth / 2, Lcd.Height-yEdge-dialogEdge-arrowHeight), new Point (Lcd.Width/ 2 + arrowWidth / 2, Lcd.Height-yEdge-dialogEdge-1));
			
        }

        protected override void OnDrawContent ()
		{
			for (int i = 0; i != lines.Count; ++i) {
				if (i + scrollPos >= options.Length)
					break;
				WriteTextOnLine(options [i + scrollPos].ToString (), i, i != cursorPos);
			}
			Lcd.DrawArrow (arrowRect, Lcd.ArrowOrientation.Down, scrollPos + lines.Count < options.Length);
			
        }

		internal override void OnUpPressed ()
		{
			if (cursorPos + scrollPos > 0) {
				if (cursorPos > 0)
					cursorPos--;
				 else 
					scrollPos--;
			} 
        }

		internal override void OnDownPressed ()
		{
			if (scrollPos + cursorPos < options.Length - 1) {
				if (cursorPos < lines.Count - 1)
					cursorPos++;
				else
					scrollPos++;
			} 
        }
        
		internal override void OnEnterPressed ()
		{
			OnExit ();
		}
		
		internal override void OnEscPressed ()
		{
			if (allowEsc) {
				EscPressed = true;
				OnExit ();
			}
		}
        
		public SelectionType GetSelection()
		{
			if(EscPressed)
				return default(SelectionType);
			else
				return options[cursorPos+scrollPos];
		}
		
		public int GetSelectionIndex ()
		{
			if(EscPressed)
				return -1;
			else
				return cursorPos+scrollPos;	
		}
		
		public bool EscPressed{get; private set;}
        
    }
}

