using System;
using MonoBrickFirmware.Display.Dialogs;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithDialog<DialogType> : IChildItem
		where DialogType : Dialog
	{

		private string title;
		private Action<DialogType> OnExit = null;
		protected bool hasFocus = false;


		public ItemWithDialog(DialogType dialog):this(dialog, "")
		{
		
		}


		public ItemWithDialog(DialogType dialog, String title)
		{
			this.title = title;
			this.Dialog = dialog;
			dialog.OnExit += OnDone;
		}

		public IParentItem Parent { get; set;}


		public DialogType Dialog{ get; protected set;}

		/// <summary>
		/// Set focus from this parent menu item
		/// </summary>
		/// <param name="menuItem">Menu item.</param>
		/// <param name="OnExit">Action to do when dialog exits</param>
		public void SetFocus(IParentItem menuItem, Action<DialogType> OnExit = null)
		{
			this.OnExit = OnExit;
			Parent = menuItem;
			hasFocus = true;
			Parent.SetFocus (this);
		}

		public void OnEnterPressed ()
		{
			if (hasFocus) 
			{
				Dialog.OnEnterPressed();	
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
				Dialog.OnLeftPressed ();
			}
		}

		public void OnRightPressed ()
		{
			if (hasFocus) 
			{
				Dialog.OnRightPressed ();
			}
		}

		public void OnUpPressed ()
		{
			if (hasFocus) 
			{
				Dialog.OnUpPressed ();
			}
		}

		public void OnDownPressed ()
		{
			if (hasFocus) 
			{
				Dialog.OnDownPressed ();
			}
		}
		public void OnEscPressed ()
		{
			if (hasFocus) 
			{
				Dialog.OnEscPressed ();
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
				Dialog.Draw ();
			}
		}
		public void OnHideContent ()
		{
			if (hasFocus)
			{
				Dialog.Hide ();
			}
		}

		private void OnDone()
		{
			Parent.RemoveFocus (this);
			hasFocus = false;
			if (OnExit != null)
			{
				OnExit (this.Dialog);
			}

		}

	}
}

