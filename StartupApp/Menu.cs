using System;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace StartupApp
{
	public interface IMenutem{
		bool EnterAction();
		void Draw(Font f, Rectangle r, bool color);
		bool LeftAction();
		bool RightAction();
	}
	
	public enum MenuItemSymbole {None, LeftArrow, RightArrow};
	
	public class MenuItemWithAction : IMenutem
	{
		private string text;
		private Lcd lcd;
		private Func<bool> action;
		private MenuItemSymbole symbole;
		private const int arrowEdge = 4;
		private const int arrowOffset = 4;
		public MenuItemWithAction(Lcd lcd, string text, Func<bool> action, MenuItemSymbole symbole = MenuItemSymbole.None){
			this.text = text;
			this.action = action;
			this.lcd = lcd;
			this.symbole = symbole;
		}
		public bool EnterAction()
		{
			return action();
		}
		public bool LeftAction (){return false;}
		public bool RightAction(){return false;}
		public void Draw (Font f, Rectangle r, bool color)
		{
			lcd.WriteTextBox (f, r, text, color);
			if (symbole == MenuItemSymbole.LeftArrow || symbole == MenuItemSymbole.RightArrow) {
				int arrowWidth =(int) f.maxWidth/3;
				Rectangle arrowRect = new Rectangle(new Point(r.P2.X -(arrowWidth+arrowOffset), r.P1.Y + arrowEdge), new Point(r.P2.X -arrowOffset, r.P2.Y-arrowEdge));
				if(symbole == MenuItemSymbole.LeftArrow)
					lcd.DrawArrow(arrowRect,Lcd.ArrowOrientation.Left, color);
				else
					lcd.DrawArrow(arrowRect,Lcd.ArrowOrientation.Right, color);
			}
		}	
	}
	
	public class MenuItemWithOptions<OptionType> : IMenutem
	{
		private string text;
		private Lcd lcd;
        private OptionType[] options;
		private const int rightArrowOffset = 4;
		private const int arrowEdge = 4;
        public MenuItemWithOptions(Lcd lcd, string text, OptionType[] options, int startIdx = 0)
        {
			this.text = text;
			this.lcd = lcd;
			this.options = options;
			this.OptionIndex = startIdx;
		}
		public bool EnterAction()
		{
			return false;
		}
		
		public bool LeftAction ()
		{
			OptionIndex = OptionIndex -1;
			if(OptionIndex < 0)
				OptionIndex = options.Length-1;
			return false;
		}
		public bool RightAction(){
			OptionIndex = (OptionIndex+1)%options.Length;
			return false;
		}
		public void Draw (Font f, Rectangle r, bool color)
		{
			int arrowWidth = (int)f.maxWidth / 4;
			
			string valueAsString = " " + options[OptionIndex].ToString() + " ";
			Point p = f.TextSize (valueAsString);
			Rectangle numericRect = new Rectangle ( new Point( Lcd.Width - p.X, r.P1.Y),r.P2);
			Rectangle textRect = new Rectangle (new Point (r.P1.X, r.P1.Y), new Point (r.P2.X - (p.X), r.P2.Y));
			Rectangle leftArrowRect = new Rectangle(new Point(numericRect.P1.X, numericRect.P1.Y+arrowEdge), new Point(numericRect.P1.X+ arrowWidth, numericRect.P2.Y-arrowEdge));
			Rectangle rightArrowRect = new Rectangle( new Point(numericRect.P2.X-(arrowWidth + rightArrowOffset), numericRect.P1.Y+arrowEdge) , new Point(numericRect.P2.X-rightArrowOffset,numericRect.P2.Y-arrowEdge));
			
			lcd.WriteTextBox (f, textRect, text, color, Lcd.Alignment.Left);
			lcd.WriteTextBox (f, numericRect, valueAsString, color, Lcd.Alignment.Right);
			lcd.DrawArrow(leftArrowRect, Lcd.ArrowOrientation.Left, color);
			lcd.DrawArrow(rightArrowRect, Lcd.ArrowOrientation.Right, color);
		}
		public int OptionIndex{get;private set;}

        OptionType GetSelection() {
            return options[OptionIndex];
        }

	}
	
	public class MenuItemWithCheckBox : IMenutem
	{
		private string text;
		private Lcd lcd;
		private const int lineSize = 2;
		private const int edgeSize = 2;
		private Func<bool,bool>func;
		
		public MenuItemWithCheckBox (Lcd lcd, string text, bool checkedAtStart, Func<bool,bool>enterFunc = null){
			this.text = text;
			this.lcd = lcd;
			this.Checked = checkedAtStart;
			this.func = enterFunc;
		}
		public bool EnterAction ()
		{
			if (func != null) {
				Checked = func(Checked);	
			} 
			else 
			{
				Checked = !Checked;
			}
			return false;
		}
		public bool LeftAction (){return false;}
		public bool RightAction(){return false;}
		public void Draw (Font f, Rectangle r, bool color)
		{
			int xCheckBoxSize =(int) f.maxWidth;
			Rectangle outer = new Rectangle(new Point(Lcd.Width - xCheckBoxSize + edgeSize, r.P1.Y + edgeSize), new Point(r.P2.X - edgeSize,r.P2.Y - edgeSize));
			Rectangle innter = new Rectangle(new Point(Lcd.Width - xCheckBoxSize + lineSize+edgeSize, r.P1.Y+lineSize + edgeSize), new Point(r.P2.X - lineSize - edgeSize,r.P2.Y - lineSize - edgeSize));
			Point fontPoint = f.TextSize("v");
			Point checkPoint = new Point(Lcd.Width - xCheckBoxSize +(int) fontPoint.X-edgeSize, r.P1.Y);
			
			lcd.WriteTextBox(f, r, text, color);
			lcd.DrawBox(outer,color);
			lcd.DrawBox(innter,!color);
			if(Checked)
				lcd.WriteText(f,checkPoint,"v", color);
		}
		public bool Checked{get;private set;}
	}
	
	public class MenuItemWithNumericInput : IMenutem
	{
		private string text;
		private Lcd lcd;
		private int min;
		private int max;
		private const int rightArrowOffset = 4;
		private const int arrowEdge = 4;
		private const int holdSleepTime = 100;
		private const int holdSingleWait = 5;
		private const int holdTenWait = 25;
		private const int holdHundredWait = 45;
		private const int holdFiveHundredWait = 75;
		
		private Font font;
		private Rectangle rect;
		private bool drawDataSaved = false;
		public MenuItemWithNumericInput (Lcd lcd, string text, int startValue, int min = int.MinValue, int max= int.MaxValue){
			this.text = text;
			this.lcd = lcd;
			this.Value = startValue;
			this.min = min;
			this.max = max;
		}
		public bool EnterAction(){
			return false;
		}
		
		public bool LeftAction ()
		{
			int counter = 0;
			using (Buttons btns = new Buttons ()) {
				Value--;
				do{
					if(counter < holdSingleWait ){
						counter++;
					}
					if(counter >= holdSingleWait  && counter < holdTenWait){
						counter++;
						Value--;
					}
					if(counter >= holdTenWait && counter < holdHundredWait){
						Value = Value -10;
						counter++;
					}
					if(counter >= holdHundredWait && counter < holdFiveHundredWait){
						Value = Value -100;
						counter++;
					}
					if(counter >= holdFiveHundredWait){
						Value=Value - 500;
					}
					if(Value<min)
						Value = max;
					this.Draw(font,rect,false);
					lcd.Update();
					System.Threading.Thread.Sleep(holdSleepTime);
				}while (btns.GetButtonStates()== Buttons.ButtonStates.Left);
			}
			return false;
		}
		
		public bool RightAction(){
			int counter = 0;
			using (Buttons btns = new Buttons ()) {
				Value++;
				do{
					if(counter < holdSingleWait ){
						counter++;
					}
					if(counter >= holdSingleWait  && counter < holdTenWait){
						counter++;
						Value++;
					}
					if(counter >= holdTenWait && counter < holdHundredWait){
						Value=Value +10;
						counter++;
					}
					if(counter >= holdHundredWait && counter < holdFiveHundredWait){
						Value = Value +100;
						counter++;
					}
					if(counter >= holdFiveHundredWait){
						Value=Value + 500;
					}
					if(Value>max)
						Value = min;
					this.Draw(font,rect,false);
					lcd.Update();
					System.Threading.Thread.Sleep(holdSleepTime);
				}while (btns.GetButtonStates()== Buttons.ButtonStates.Right);
			}
			return false;
		}
		public void Draw (Font f, Rectangle r, bool color)
		{
			if (!drawDataSaved) 
			{
				font = f;
				rect = r;
				drawDataSaved = true;
			}
			
			int arrowWidth = (int)f.maxWidth / 4;
			
			string valueAsString = " " + Value.ToString () + " ";
			Point p = f.TextSize (valueAsString);
			Rectangle numericRect = new Rectangle ( new Point( Lcd.Width - p.X, r.P1.Y),r.P2);
			Rectangle textRect = new Rectangle (new Point (r.P1.X, r.P1.Y), new Point (r.P2.X - (p.X), r.P2.Y));
			Rectangle leftArrowRect = new Rectangle(new Point(numericRect.P1.X, numericRect.P1.Y+arrowEdge), new Point(numericRect.P1.X+ arrowWidth, numericRect.P2.Y-arrowEdge));
			Rectangle rightArrowRect = new Rectangle( new Point(numericRect.P2.X-(arrowWidth + rightArrowOffset), numericRect.P1.Y+arrowEdge) , new Point(numericRect.P2.X-rightArrowOffset,numericRect.P2.Y-arrowEdge));
			
			lcd.WriteTextBox (f, textRect, text, color, Lcd.Alignment.Left);
			lcd.WriteTextBox (f, numericRect, valueAsString, color, Lcd.Alignment.Right);
			lcd.DrawArrow(leftArrowRect, Lcd.ArrowOrientation.Left, color);
			lcd.DrawArrow(rightArrowRect, Lcd.ArrowOrientation.Right, color);
		}
		public int Value{get;private set;}
	}
	
	
	
	public class Menu
	{
		IMenutem[] items;
		Lcd lcd;
		Font font;
		string title;
		Point itemSize;
		Point itemHeight;
		int itemsOnScreen;
		int cursorPos;
		int scrollPos;
		Buttons btns;
		public Menu (Font f, Lcd lcd, Buttons btns, string title, IEnumerable<IMenutem> items)
		{
			this.font = f;
			this.lcd = lcd;
			this.title = title;
			this.items = items.ToArray();			
			this.itemSize = new Point(Lcd.Width, (int)font.maxHeight);
			this.itemHeight = new Point(0, (int)font.maxHeight);
			this.itemsOnScreen = (int)(Lcd.Height/font.maxHeight - 1); // -2 Because of the title and arrows
			this.btns = btns;
			cursorPos = 0;
			scrollPos = 0;
		}
		
		private void RedrawMenu ()
		{
			lcd.Clear ();
			Rectangle startPos = new Rectangle (new Point (0, 0), itemSize);
			
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

