using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Display.Dialogs;

namespace MonoBrickFirmware.Display.Menus
{
	public class  MenuItemWithCharacterInput : IMenuItem
	{
		private string subject;
		private string dialogTitle;
		private const int lineSize = 2;
		private const int edgeSize = 2;
		private bool hide;
		public Action<Dialogs.CharacterDialog> OnShowDialog = delegate {};
		public Action<string> OnDialogExit = delegate {};
		public  MenuItemWithCharacterInput (string subject, string dialogTitle, string startText, bool hideInput = false){
			this.dialogTitle = dialogTitle; 
			this.subject = subject;
			this.Text = startText;
			this.hide = hideInput;
		}
		public bool EnterAction ()
		{
			var dialog = new Dialogs.CharacterDialog(dialogTitle);
			dialog.OnShow += delegate{this.OnShowDialog(dialog);};
			dialog.OnExit += delegate{Text = dialog.GetUserInput();this.OnDialogExit(Text);};
			dialog.Show();
			return false;
		}
		public bool LeftAction (){return false;}
		public bool RightAction(){return false;}
		public void Draw (Font f, Rectangle r, bool color)
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

		public string Text{get;private set;}
	}
}

