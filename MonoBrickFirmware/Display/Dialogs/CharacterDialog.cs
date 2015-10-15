using System;
using System.Collections.Generic;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Display.Dialogs.UserInput;

namespace MonoBrickFirmware.Display.Dialogs
{
	
	public class CharacterDialog : Dialog
    {
		private Keyboard keyBoard;
		private bool okWithEsc = false;
		private bool frameHasBeenDrawn = false;
		public CharacterDialog(string title, bool disableNumbersAndSymbols, bool disableEnter) : base(Font.MediumFont, title, Lcd.Width, Lcd.Height-14) 
        {
			keyBoard = new Keyboard(this.innerWindow, disableEnter,disableNumbersAndSymbols);
			keyBoard.OnOk += () => {this.OnExit(); frameHasBeenDrawn = false;};
			keyBoard.OnCancel += () => {okWithEsc = true; OnExit();frameHasBeenDrawn = false;};
        }
        
		public string GetUserInput ()
		{
			return okWithEsc ? null : keyBoard.Text;
		}

		internal override void OnEscPressed ()
		{
			keyBoard.Esc ();
		}

		internal override void OnEnterPressed ()
		{
			keyBoard.Enter ();	
		}

		internal override void OnLeftPressed ()
		{
			keyBoard.Left();	
		}
		
		internal override void OnRightPressed ()
		{
			keyBoard.Right();
		}
		
		internal override void OnUpPressed ()
		{
			keyBoard.Up();
		}
		
		internal override void OnDownPressed ()
		{
			keyBoard.Down();
		}

		protected override void OnDrawContent ()
		{
			keyBoard.Draw ();
        }

		internal override void Draw ()
		{
			OnShow ();
			if (!frameHasBeenDrawn)
			{
				Lcd.DrawRectangle(outherWindow, true, true);
				Lcd.DrawRectangle(innerWindow, false, true);
				if (drawTitle)
				{
					WriteTitle ();
				}
				frameHasBeenDrawn = true;
			}
			OnDrawContent();
			Lcd.Update();
		}
    }
}

