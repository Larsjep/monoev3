using System;
using MonoBrickFirmware.Display;
 
namespace MonoBrickFirmware.Display.Menus
{
	
	public interface IChildItem
	{
		/// <summary>
		/// The action to do when enter is pressed
		/// </summary>
		void OnEnterPressed();

		/// <summary>
		/// The action to do when left is pressed
		/// </summary>
		void OnLeftPressed();

		/// <summary>
		/// The action to do when right is pressed
		/// </summary>
		void OnRightPressed();

		/// <summary>
		/// The action to do when up is pressed
		/// </summary>
		void OnUpPressed();

		/// <summary>
		/// The action to do when down is pressed
		/// </summary>
		void OnDownPressed();

		/// <summary>
		/// The action to do when esc is pressed
		/// </summary>
		void OnEscPressed();

		/// <summary>
		/// Draws the menu item.
		/// </summary>
		/// <param name="font">Font to use.</param>
		/// <param name="rectangle">Rectangle available for the menu menu item title</param>
		/// <param name="color">If set to <c>true</c> item is selected.</param>
		void OnDrawTitle(Font font, Rectangle rectangle, bool selected);

		/// <summary>
		/// Draw the content of the menu item. This is only called when the item has focus
		/// </summary>
		void OnDrawContent();

		/// <summary>
		/// Hide the menu item content 
		/// </summary>
		void OnHideContent();

		/// <summary>
		/// Parent menu item
		/// </summary>
		IParentItem Parent{get; set;}
	}




}

