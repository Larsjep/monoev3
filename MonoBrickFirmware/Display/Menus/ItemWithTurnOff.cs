using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Device;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithTurnOff : ChildItemWithParent
	{
		private ItemWithDialog<QuestionDialog> questionDialog = new ItemWithDialog<QuestionDialog> (new QuestionDialog ("Are you sure?", "Shutdown EV3")); 
		public ItemWithTurnOff () : base("Shutdown")
		{
			
		}

		public override void OnEnterPressed ()
		{
			questionDialog.SetFocus (this, OnExit);	
		}

		private void OnExit (QuestionDialog dialog)
		{
			if (dialog.IsPositiveSelected) 
			{
				Parent.SuspendButtonEvents ();
				Lcd.Clear();
				Lcd.WriteText(Font.MediumFont, new Point(0,0), "Shutting down...", true);
				Lcd.Update();

				Buttons.LedPattern(2);
				Brick.TurnOff ();
			} 
		}
	}
}

