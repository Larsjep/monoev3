using System;
using MonoBrickFirmware.Display.Dialogs;

namespace MonoBrickFirmware.Display.Menus
{
	public abstract class ItemWithDialog<DialogType> : IChildItem
		where DialogType : Dialog
	{

		private string title;
		protected bool hasFocus = false;
		protected DialogType dialog;

		public abstract void OnExit (DialogType dialog);

		public ItemWithDialog(DialogType dialog):this(dialog, "")
		{
		
		}


		public ItemWithDialog(DialogType dialog, String title)
		{
			this.title = title;
			this.dialog = dialog;
			dialog.OnExit += OnDone;
		}

		public IParentItem Parent { get; set;}


		/// <summary>
		/// Set focus from this parent menu item
		/// </summary>
		/// <param name="menuItem">Menu item.</param>
		public void SetFocus(IParentItem menuItem)
		{
			this.Parent = menuItem;
			hasFocus = true;
			Parent.SetFocus (this);
		}


		public void OnEnterPressed ()
		{
			if (hasFocus) 
			{
				dialog.OnEnterPressed();	
			} 
			else 
			{
				hasFocus = true;
				Parent.SetFocus (this);
			}

		}

		public void OnLeftPressed ()
		{
			if (hasFocus) 
			{
				dialog.OnLeftPressed ();
			}
		}

		public void OnRightPressed ()
		{
			if (hasFocus) 
			{
				dialog.OnRightPressed ();
			}
		}

		public void OnUpPressed ()
		{
			if (hasFocus) 
			{
				dialog.OnUpPressed ();
			}
		}

		public void OnDownPressed ()
		{
			if (hasFocus) 
			{
				dialog.OnDownPressed ();
			}
		}
		public void OnEscPressed ()
		{
			if (hasFocus) 
			{
				dialog.OnEscPressed ();
			}
		}

		public void OnDrawTitle (Font font, Rectangle rectangle, bool selected)
		{
			Lcd.WriteTextBox (font, rectangle, title, selected);
		}

		public void OnDrawContent ()
		{
			if (hasFocus) 
			{
				dialog.Draw ();
			}
		}
		public void OnHideContent ()
		{
			if (hasFocus)
			{
				dialog.Hide ();
			}
		}

		private void OnDone()
		{
			Parent.RemoveFocus (this);
			OnExit(this.dialog);
		}

	}
}

