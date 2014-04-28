using System;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Display.Menus
{
	
	public class Menu
	{
		IMenuItem[] menuItems;
		Font font;
		string title;
		Point itemSize;
		Point itemHeight;
		int itemsOnScreen;
		int cursorPos;
		int scrollPos;
		Buttons btns;
		int arrowHeight = 5;
		int arrowWidth = 10;
		
		public Menu (Font f, Buttons btns, string title, IEnumerable<IMenuItem> items)
		{
			this.font = f;
			this.title = title;
			this.menuItems = items.ToArray ();			
			this.itemSize = new Point (Lcd.Width, (int)font.maxHeight);
			this.itemHeight = new Point (0, (int)font.maxHeight);
			this.itemsOnScreen = (int)((Lcd.Height-arrowHeight)/ font.maxHeight - 1); // -1 Because of the title
			this.btns = btns;
			cursorPos = 0;
			scrollPos = 0;
		}
		
		private void RedrawMenu ()
		{
			Lcd.Instance.Clear ();
			Rectangle currentPos = new Rectangle (new Point (0, 0), itemSize);
			Rectangle arrowRect = new Rectangle (new Point (Lcd.Width / 2 - arrowWidth / 2, Lcd.Height - arrowHeight), new Point (Lcd.Width / 2 + arrowWidth / 2, Lcd.Height-1));

			Lcd.Instance.WriteTextBox (font, currentPos, title, true, Lcd.Alignment.Center);
			int i = 0;
			while (i != itemsOnScreen) {
				if (i + scrollPos >= menuItems.Length)
					break;
				menuItems [i + scrollPos].Draw (font, currentPos + itemHeight * (i + 1), i != cursorPos);
				i++;
			}
			Lcd.Instance.DrawArrow (arrowRect, Lcd.ArrowOrientation.Down, scrollPos + itemsOnScreen < menuItems.Length);
			Lcd.Instance.Update();
		}
		
		private void MoveUp()
		{
			if (cursorPos+scrollPos > 0)
			{
				if (cursorPos > 0)
					cursorPos--;
				else
					scrollPos--;
			}
		}
		
		private void MoveDown()
		{
			if (scrollPos+cursorPos < menuItems.Length-1)
			{
				if (cursorPos < itemsOnScreen-1)
					cursorPos++;
				else
					scrollPos++;
			}
		}
		
		public void Show()
		{
			bool exit = false;
			while (!exit)
			{
			  	RedrawMenu();
				switch (btns.GetKeypress())
				{
					case Buttons.ButtonStates.Down: 
					  MoveDown();
					break;
					case Buttons.ButtonStates.Up:
					  MoveUp();
					break;
					case Buttons.ButtonStates.Escape:
					  exit = true;
					break;
					case Buttons.ButtonStates.Enter:
						if (menuItems[scrollPos+cursorPos].EnterAction())
				   		{
					    	exit = true;
						}
					break;
					case Buttons.ButtonStates.Left:
						if (menuItems[scrollPos+cursorPos].LeftAction())
				   		{
					    	exit = true;
						}
					break;
					case Buttons.ButtonStates.Right:
						if (menuItems[scrollPos+cursorPos].RightAction())
				   		{
					    	exit = true;
						}
					break;
				}
			}
			
		}
	}
}

