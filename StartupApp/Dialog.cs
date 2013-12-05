using System;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace StartupApp
{
		
	public interface IDialogItem{
		bool EnterAction();
		void Draw(Font f, Rect r, bool color);
		bool LeftAction();
		bool RightAction();
		bool UpAction();
		bool DownAction();
		bool Escape();
	}
	
	public class NumericDialogItem:IDialogItem
	{
		private Lcd lcd;
		public NumericDialogItem (Lcd lcd, int startValue)
		{
			Value = startValue;
			this.lcd = lcd;
		}
		
		public bool EnterAction()
		{
			return false;												
		}
		
		public void Draw(Font f, Rect r, bool color)
		{
			lcd.WriteTextBox(f,r,Value.ToString(),color,Lcd.Alignment.Center);															
		}
		
		public bool LeftAction ()
		{
			return false;												
		}
		
		public bool RightAction()
		{
			return false;												
		}
		
		public bool UpAction()
		{
			return false;												
		}
		
		public bool DownAction()
		{
			return false;												
		}
		
		public bool Escape()
		{
			return true;												
		}
		
		public int Value{get;private set;}
	}
	
	
	public class Dialog
	{
		IDialogItem item;
		Lcd lcd;
		Font font;
		string title;
		int titleSize;
		int itemSize;
		int itemHeight;
		Buttons btns;
		const float dialogHeightPct = 0.60f;
		const float dialogWidthPct = 0.90f;
		const int dialogEdge = 5;
		Rect dialogWindowOuther; 
		Rect dialogWindowInner;
		Rect itemWindow;
		Rect titleRect;
		public Dialog (Font f, Lcd lcd, Buttons btns, string title, IDialogItem dialogItem)
		{
			this.item = dialogItem;
			this.font = f;
			this.lcd = lcd;
			this.title = title;
			this.btns = btns;
			int xEdge = (int)((float) (Lcd.Width) * (1.0 - dialogWidthPct)/2);
			int yEdge = (int)((float) (Lcd.Height) * (1.0 - dialogHeightPct)/2);
			int ySize = (int) ((float) (Lcd.Height) * dialogHeightPct);
			int xSize = (int)((float) (Lcd.Width) * dialogWidthPct);
			Point startPoint1 = new Point(xEdge,yEdge);
			Point startPoint2 = new Point(xEdge + xSize, yEdge + ySize);
			this.itemHeight = (int)font.maxHeight;
			this.itemSize = (startPoint2.X - dialogEdge) - (startPoint1.X + dialogEdge);
			int yItem = (int)(Lcd.Height/2) - (int)(font.maxHeight/2);
			int xItem = startPoint1.X + dialogEdge;
			Point itemPoint1 = new Point(xItem,yItem);
			Point itemPoint2 = new Point(xItem + itemSize, yItem+itemHeight);
			this.titleSize = font.TextSize(this.title).X;
			dialogWindowOuther = new Rect(startPoint1, startPoint2);
			dialogWindowInner = new Rect(new Point(startPoint1.X + dialogEdge, startPoint1.Y+dialogEdge), new Point(startPoint2.X-dialogEdge, startPoint2.Y-dialogEdge));
			itemWindow = new Rect(itemPoint1,itemPoint2);
			titleRect = new Rect(new Point((int)( Lcd.Width/2 - titleSize/2), (int)(startPoint1.Y - (font.maxHeight/2)) ), new Point((int)( Lcd.Width/2 + titleSize/2),(int)( startPoint1.Y + (font.maxHeight/2)) ));
		}
		
		private void RedrawDialog ()
		{
			lcd.DrawBox(dialogWindowOuther, true);
			lcd.DrawBox(dialogWindowInner, false);
			item.Draw(this.font,itemWindow, true);
			lcd.WriteTextBox(font,titleRect,title, false,Lcd.Alignment.Center); 
			lcd.Update();
		}
		
		public void Show ()
		{
			bool exit = false;
			while (!exit) {
				RedrawDialog ();
				switch (btns.GetKeypress ()) {
				case Buttons.ButtonStates.Down: 
					if (item.DownAction()) 
					{
						exit = true;
					}
					break;
				case Buttons.ButtonStates.Up:
					if (item.UpAction ()) 
					{
						exit = true;
					}
					break;
				case Buttons.ButtonStates.Escape:
					if (item.Escape()) 
					{
						exit = true;
					}
					exit = true;
					break;
				case Buttons.ButtonStates.Enter:
					if (item.EnterAction ()) 
					{
						exit = true;
					}
					break;
				case Buttons.ButtonStates.Left:
					if (item.LeftAction()) 
					{
						exit = true;
					}
					break;
				case Buttons.ButtonStates.Right:
					if (item.RightAction()) 
					{
						exit = true;
					}
					break;
				}
			}
			
		}
	}
}

