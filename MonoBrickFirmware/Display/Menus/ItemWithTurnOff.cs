using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Device;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithTurnOff : ItemWithDialog<QuestionDialog>
	{
		public ItemWithTurnOff () : base (new QuestionDialog ("Are you sure?", "Shutdown EV3"), "Shutdown")
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
				Brick.TurnOff ();
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

