using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Display.Dialogs;
using System.Threading;

namespace MonoBrickFirmware.Display.Menus
{
	public class  ItemWithCharacterInput : IChildItem
	{
		private string subject;
		private string dialogTitle;
		private const int lineSize = 2;
		private const int edgeSize = 2;
		private bool hide;
		private bool show = false;
		private CharacterDialog dialog; 
		public Action<CharacterDialog> OnShowDialog = delegate {};
		public Action<string> OnDialogExit = delegate {};

		public  ItemWithCharacterInput (string subject, string dialogTitle, string startText, bool hideInput = false){
			this.dialogTitle = dialogTitle; 
			this.subject = subject;
			this.Text = startText;
			this.hide = hideInput;
			dialog = new Dialogs.CharacterDialog(dialogTitle);
			dialog.OnShow += delegate{this.OnShowDialog(dialog);};
			dialog.OnExit += delegate{Text = dialog.GetUserInput();this.OnDialogExit(Text); show = false; Parent.RemoveFocus(this);};
		}

		public IParentItem Parent { get; set;}

		public void OnEnterPressed ()
		{
			if (show)
			{
				dialog.OnEnterPressed ();
			} 
			else 
			{
				show = true;
				Parent.SetFocus (this);
			}
		}

		public void OnDrawTitle(Font f, Rectangle r, bool color)
		{
			string showTextString;
			int totalWidth = r.P2.X - r.P1.X;
			int subjectWidth = (int)(f.TextSize (subject + "  ").X);
			int textValueWidth = totalWidth - subjectWidth;
			Rectangle textRect = new Rectangle (new Point (r.P1.X + subjectWidth, r.P1.Y), r.P2);
			Rectangle subjectRect = new Rectangle (r.P1, new Point (r.P2.X - textValueWidth, r.P2.Y));
			if ((int)(f.TextSize (Text).X) < textValueWidth) {
				showTextString = Text;
				if (hide) {
					showTextString = new string ('*', showTextString.Length); 
				}	
			} else {
				
				showTextString = "";
				for (int i = 0; i < Text.Length; i++) {
					if (f.TextSize (showTextString + this.Text [i] + "...").X < textValueWidth) {
						showTextString = showTextString + Text [i];
					} else {
						break;
					} 
				}
				if (hide) 
				{
					showTextString = new string ('*', showTextString.Length); 
				} 
				else 
				{
					showTextString = showTextString + "...";
				}
			}
			Lcd.Instance.WriteTextBox (f, subjectRect,subject + "  ", color);
			Lcd.Instance.WriteTextBox(f,textRect,showTextString,color,Lcd.Alignment.Right);
		}

		public void OnDrawContent ()
		{
			dialog.Draw ();		
		}

		public void OnUpPressed ()
		{
			if (show)
			{
				dialog.OnUpPressed ();		
			}
		}

		public void OnDownPressed ()
		{
			if (show)
			{
				dialog.OnDownPressed();		
			}
		}

		public void OnEscPressed ()
		{
			if (show)
			{
				dialog.OnEscPressed ();		
			}			
		}

		public void OnLeftPressed ()
		{
			if (show)
			{
				dialog.OnLeftPressed ();	
			}
		}

		public void OnRightPressed()
		{
			if (show)
			{
				dialog.OnRightPressed ();	
			}	
		}

		public void OnHideContent()
		{
			if (show) 
			{
				dialog.Hide ();
			}
		}

		public string Text{get;private set;}
	}
}

