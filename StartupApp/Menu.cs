using System;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace StartupApp
{
	public struct MenuItem
	{
		public string text;
		public Action action;
	}
	
	public class Menu
	{
		MenuItem[] items;
		Lcd lcd;
		Font font;
		string title;
		Point itemSize;
		Point itemHeight;
		int itemsOnScreen;
		int cursorPos;
		int scrollPos;
		
		public Menu (Font f, Lcd lcd, string title, IEnumerable<MenuItem> items)
		{
			this.font = f;
			this.lcd = lcd;
			this.title = title;
			this.items = items.ToArray();			
			this.itemSize = new Point(Lcd.Width, (int)font.maxHeight);
			this.itemHeight = new Point(0, (int)font.maxHeight);
			this.itemsOnScreen = (int)(Lcd.Height/font.maxHeight - 1); // -1 Because of the title
			cursorPos = 0;
			scrollPos = 0;
		}
		
		private void RedrawMenu()
		{
			lcd.Clear();
			Rect startPos = new Rect(new Point(0,0), itemSize);
			
			lcd.WriteTextBox(font, startPos, title, true, Lcd.Alignment.Center);
			
			for (int i = 0; i != itemsOnScreen; ++i)
			{
				if (i+scrollPos >= items.Length)
					break;
				lcd.WriteTextBox(font, startPos+itemHeight*(i+1), items[i+scrollPos].text, i != cursorPos);
			}
			lcd.Update();
		}
		
		void MoveUp()
		{
			if (cursorPos+scrollPos > 0)
			{
				if (cursorPos > 0)
					cursorPos--;
				else
					scrollPos--;
			}
		}
		
		void MoveDown()
		{
			if (scrollPos+cursorPos < items.Length-1)
			{
				if (cursorPos < itemsOnScreen-1)
					cursorPos++;
				else
					scrollPos++;
			}
		}
		
		
		public void ShowMenu(Buttons btns)
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
					  items[scrollPos+cursorPos].action();
					break;
				}
			}
			
		}
	}
}

