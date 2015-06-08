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
		/// Susspends events from being fired
		/// </summary>
		void SuspendEvents(IChildItem item);

		/// <summary>
		/// Allow events to be fired
		/// </summary>
		void ResumeEvents(IChildItem item);
	}
}

