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
				Lcd.Instance.Clear();
				Lcd.Instance.WriteText(Font.MediumFont, new Point(0,0), "Shutting down...", true);
				Lcd.Instance.Update();

				Buttons.Instance.LedPattern(2);
				ProcessHelper.RunAndWaitForProcess("/sbin/shutdown", "-h now");
				Thread.Sleep(120000);
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

