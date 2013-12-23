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
		
		public SelectDialog (Font f, Lcd lcd, Buttons btns, SelectionType[] options, string title, bool allowEsc) : base (f, lcd, btns, title, 160,90+(int)f.maxHeight/2,(int)f.maxHeight/4)
		{
			this.options = options;
			cursorPos = 0;
			scrollPos = 0;
			this.allowEsc = allowEsc;
			EscPressed = false;
        }

        protected override void OnDrawContent ()
		{
			for (int i = 0; i != lines.Count; ++i) {
				if (i + scrollPos >= options.Length)
					break;
				lcd.WriteTextBox (font, lines [i], options [i + scrollPos].ToString (), i != cursorPos, Lcd.Alignment.Center); 	
			}
        }

        protected override bool OnUpAction ()
		{
			if (cursorPos + scrollPos > 0) {
				if (cursorPos > 0)
					cursorPos--;
				 else 
					scrollPos--;
			} 
			return false;
        }

        protected override bool OnDownAction ()
		{
			if (scrollPos + cursorPos < options.Length - 1) {
				if (cursorPos < lines.Count - 1)
					cursorPos++;
				else
					scrollPos++;
			} 
           	return false;
        }
        
        protected override bool OnEnterAction ()
		{
			return true;
		}
		
		protected override bool OnEscape ()
		{
			if (allowEsc) {
				EscPressed = true;
				return true;
			}
			return false;
		}
        
		public SelectionType GetSelection()
		{
			if(EscPressed)
				return default(SelectionType);
			else
				return options[cursorPos+scrollPos];
		}
		
		public bool EscPressed{get; private set;}
        
    }
}

