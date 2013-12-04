using System;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace StartupApp
{
	public interface IMenuItem{
		bool EnterAction();
		void Draw(Font f, Rect r, bool color);
		bool LeftAction();
		bool RightAction();
	}
	
	public class MenuItem : IMenuItem
	{
		private string text;
		private Lcd lcd;
		private Func<bool> action;
		public MenuItem(Lcd lcd, string text, Func<bool> action){
			this.text = text;
			this.action = action;
			this.lcd = lcd;
		}
		public bool EnterAction()
		{
			return action();
		}
		public bool LeftAction (){return false;}
		public bool RightAction(){return false;}
		public void Draw (Font f, Rect r, bool color)
		{
			lcd.WriteTextBox(f, r, text, color);	
		}	
	}
	
	public class MenuItemWithOptions : IMenuItem
	{
		private string text;
		private Lcd lcd;
		private string[] options;
		public MenuItemWithOptions(Lcd lcd, string text, string[] options, int startIdx = 0){
			this.text = text;
			this.lcd = lcd;
			this.options = options;
			this.OptionIndex = startIdx;
		}
		public bool EnterAction()
		{
			OptionIndex = (OptionIndex+1)%options.Length;
			return false;
		}
		public bool LeftAction (){return false;}
		public bool RightAction(){return false;}
		public void Draw (Font f, Rect r, bool color)
		{
			lcd.WriteTextBox(f, r, options[OptionIndex], color, Lcd.Alignment.Right);
			lcd.WriteText(f, new Point (0, 0) + r.P1, text, color);
		}
		public int OptionIndex{get;private set;}
	}
	
	public class MenuItemWithCheck : IMenuItem
	{
		private string text;
		private Lcd lcd;
		private int xCheckBoxSize;
		private const int lineSize = 2;
		private const int edgeSize = 2;
		
		public MenuItemWithCheck (Lcd lcd, string text, bool checkedAtStart){
			this.text = text;
			this.lcd = lcd;
			this.Checked = checkedAtStart;
		}
		public bool EnterAction()
		{
			Checked = !Checked;
			return false;
		}
		public bool LeftAction (){return false;}
		public bool RightAction(){return false;}
		public void Draw (Font f, Rect r, bool color)
		{
			xCheckBoxSize =(int) f.maxWidth;
			Rect outer = new Rect(new Point(Lcd.Width - xCheckBoxSize + edgeSize, r.P1.Y + edgeSize), new Point(r.P2.X - edgeSize,r.P2.Y - edgeSize));
			Rect innter = new Rect(new Point(Lcd.Width - xCheckBoxSize + lineSize+edgeSize, r.P1.Y+lineSize + edgeSize), new Point(r.P2.X - lineSize - edgeSize,r.P2.Y - lineSize - edgeSize));
			Point fontPoint = f.TextSize("v");
			Point checkPoint = new Point(Lcd.Width - xCheckBoxSize +(int) fontPoint.X-edgeSize, r.P1.Y);
			
			lcd.WriteTextBox(f, r, text, color);
			lcd.DrawBox(outer,color);
			lcd.DrawBox(innter,!color);
			if(Checked)
				lcd.WriteText(f,checkPoint,"v", color);
			
			
			
			
			//lcd.WriteText(f, new Point (0, 0) + r.p1, text, color);
			
		}
		public bool Checked{get;private set;}
	}
	
	
	public class Menu
	{
		IMenuItem[] items;
		Lcd lcd;
		Font font;
		string title;
		Point itemSize;
		Point itemHeight;
		int itemsOnScreen;
		int cursorPos;
		int scrollPos;
		
		public Menu (Font f, Lcd lcd, string title, IEnumerable<IMenuItem> items)
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
		
		private void RedrawMenu ()
		{
			lcd.Clear ();
			Rect startPos = new Rect (new Point (0, 0), itemSize);
			
			lcd.WriteTextBox (font, startPos, title, true, Lcd.Alignment.Center);
			
			for (int i = 0; i != itemsOnScreen; ++i) {
				if (i + scrollPos >= items.Length)
					break;
				items[i + scrollPos].Draw(font, startPos+itemHeight*(i+1), i != cursorPos);
			}
			lcd.Update();
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
						if (items[scrollPos+cursorPos].EnterAction())
				   		{
					    	exit = true;
						}
					break;
					case Buttons.ButtonStates.Left:
						if (items[scrollPos+cursorPos].LeftAction())
				   		{
					    	exit = true;
						}
					break;
					case Buttons.ButtonStates.Right:
						if (items[scrollPos+cursorPos].RightAction())
				   		{
					    	exit = true;
						}
					break;
				}
			}
			
		}
	}
}

