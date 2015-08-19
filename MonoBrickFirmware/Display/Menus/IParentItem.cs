using System;

namespace MonoBrickFirmware.Display.Menus
{
	public interface IParentItem
	{
		/// <summary>
		/// Sets focus on this item 
		/// </summary>
		/// <param name="item">Item to set focus on</param>
		void SetFocus(IChildItem item);

		/// <summary>
		/// Removes focus from this item.
		/// </summary>
		/// <param name="item">Item to remove focus from</param>
		void RemoveFocus(IChildItem item);

		/// <summary>
		/// Susspends button events from being fired
		/// </summary>
		void SuspendButtonEvents();

		/// <summary>
		/// Allow button events to be fired
		/// </summary>
		void ResumeButtonEvents();
	}
}

