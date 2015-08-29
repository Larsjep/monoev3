using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Device;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithTurnOff : IChildItem, IParentItem
	{
		private ItemWithDialog<QuestionDialog> questionDialog = new ItemWithDialog<QuestionDialog> (new QuestionDialog ("Are you sure?", "Shutdown EV3")); 
		public ItemWithTurnOff ()
		{
			
		}

		public void OnEnterPressed ()
		{
			questionDialog.SetFocus (this, OnExit);	
		}
		public void OnLeftPressed ()
		{
			
		}
		public void OnRightPressed ()
		{
			
		}
		public void OnUpPressed ()
		{
			
		}
		public void OnDownPressed ()
		{
			
		}
		public void OnEscPressed ()
		{
			
		}
		public void OnDrawTitle (Font font, Rectangle rectangle, bool selected)
		{
			Lcd.WriteTextBox (font, rectangle, "Shutdown", selected);

		}

		public void OnDrawContent ()
		{
			
		}
		public void OnHideContent ()
		{
			
		}
		public IParentItem Parent { get; set; }

		public void SetFocus (IChildItem item)
		{
			Parent.SetFocus (item);	
		}

		public void RemoveFocus (IChildItem item)
		{
			Parent.RemoveFocus (item);
		}

		public void SuspendButtonEvents ()
		{
			Parent.SuspendButtonEvents ();	
		}

		public void ResumeButtonEvents ()
		{
			Parent.ResumeButtonEvents ();	
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

