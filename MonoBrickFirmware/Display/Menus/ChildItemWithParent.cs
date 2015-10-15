using System;

namespace MonoBrickFirmware.Display.Menus
{
	public class ChildItemWithParent : ChildItem, IParentItem
	{
		public ChildItemWithParent(string title) : base(title)
		{
			
		}

		public virtual void SetFocus (IChildItem item)
		{
			Parent.SetFocus (item);
		}
		public virtual void RemoveFocus (IChildItem item)
		{
			Parent.RemoveFocus (item);
		}
		public virtual void SuspendButtonEvents ()
		{
			Parent.SuspendButtonEvents ();
		}
		public virtual void ResumeButtonEvents ()
		{
			Parent.ResumeButtonEvents ();
		}
	}
}

