using System;

namespace MonoBrickFirmware.Display.Menus
{
	public class ChildItem : IChildItem
	{
		private string title;
		public ChildItem (): this("")
		{
			
		}

		public ChildItem (string title)
		{
			this.title = title;
		}

		public virtual void OnEnterPressed ()
		{
			
		}

		public virtual void OnLeftPressed ()
		{
			
		}

		public virtual void OnRightPressed ()
		{
			
		}

		public virtual void OnUpPressed ()
		{
			
		}

		public virtual void OnDownPressed ()
		{
			
		}

		public virtual void OnEscPressed ()
		{
			
		}

		public virtual void OnDrawTitle (Font font, Rectangle rectangle, bool selected)
		{
			Lcd.WriteTextBox (font, rectangle, title, selected);	
		}

		public virtual void OnDrawContent ()
		{
			
		}

		public virtual void OnHideContent ()
		{
			
		}

		public IParentItem Parent { get; set;}

	}
}

