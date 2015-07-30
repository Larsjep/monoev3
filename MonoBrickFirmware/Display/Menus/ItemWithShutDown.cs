using System;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Native;
using System.Threading;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithShutDown : ItemWithDialog<QuestionDialog>
	{
		public ItemWithShutDown () : base (new QuestionDialog ("Are you sure?", "Shutdown EV3"), "Shutdown")
		{
			
		}

		public override void OnExit (QuestionDialog dialog)
		{
			if (dialog.IsPositiveSelected) 
			{
				Lcd.Clear();
				Lcd.WriteText(Font.MediumFont, new Point(0,0), "Shutting down...", true);
				Lcd.Update();

				Buttons.LedPattern(2);
				SystemCalls.ShutDown ();
				Parent.RemoveFocus (this);
			} 
			else 
			{
				this.hasFocus = false;
				Parent.RemoveFocus (this);
			}
		}
	}
}

