using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace StartupApp
{
	
	public class InfoDialog: Dialog{
		public InfoDialog(Font f, Lcd lcd, Buttons btns, string message):base(f,lcd,btns,"Information", new InfoDialogItem(lcd, message)){
		
		}
		public void UpdateMessage(string message){
			((InfoDialogItem)this.dialogItem).Message = message;
			this.Redraw();
		}
	}
	
	public interface IDialogItem{
		bool EnterAction();
		void Draw(Font f, Rectangle r, bool color);
		bool LeftAction();
		bool RightAction();
		bool UpAction();
		bool DownAction();
		bool Escape();
	}
	
	internal class InfoDialogItem:IDialogItem
	{
		private Lcd lcd;
		public InfoDialogItem (Lcd lcd, string startMessage)
		{
			Message = startMessage;
			this.lcd = lcd;
		}
		
		public bool EnterAction()
		{
			return false;												
		}
		
		public void Draw(Font f, Rectangle r, bool color)
		{
			lcd.WriteTextBox(f,r,Message,color,Lcd.Alignment.Center);															
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
			return false;												
		}
		
		public string Message{get;set;}
	}
	
	
	public class Dialog
	{
		protected IDialogItem dialogItem;
		private Lcd lcd;
		private Font font;
		private string title;
		private int titleSize;
		private int itemSize;
		private int itemHeight;
		private Buttons btns;
		private const float dialogHeightPct = 0.60f;
		private const float dialogWidthPct = 0.90f;
		private const int dialogEdge = 5;
		private Rectangle dialogWindowOuther; 
		private Rectangle dialogWindowInner;
		private Rectangle itemWindow;
		private Rectangle titleRect;
		private bool close = false;
		private ManualResetEvent inputThreadDone;
		public Dialog (Font f, Lcd lcd, Buttons btns, string title, IDialogItem dialogItem)
		{
			this.dialogItem = dialogItem;
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
			this.titleSize = font.TextSize(this.title).X + (int)f.maxWidth;
			dialogWindowOuther = new Rectangle(startPoint1, startPoint2);
			dialogWindowInner = new Rectangle(new Point(startPoint1.X + dialogEdge, startPoint1.Y+dialogEdge), new Point(startPoint2.X-dialogEdge, startPoint2.Y-dialogEdge));
			itemWindow = new Rectangle(itemPoint1,itemPoint2);
			titleRect = new Rectangle(new Point((int)( Lcd.Width/2 - titleSize/2), (int)(startPoint1.Y - (font.maxHeight/2)) ), new Point((int)( Lcd.Width/2 + titleSize/2),(int)( startPoint1.Y + (font.maxHeight/2)) ));
		}
		
		protected void Redraw ()
		{
			lcd.DrawBox(dialogWindowOuther, true);
			lcd.DrawBox(dialogWindowInner, false);
			dialogItem.Draw(this.font,itemWindow, true);
			lcd.WriteTextBox(font,titleRect,title, false,Lcd.Alignment.Center); 
			lcd.Update();
		}
		
		private Buttons.ButtonStates WaitForInput ()
		{
			Buttons.ButtonStates bs = btns.GetButtonStates();
			while (bs != Buttons.ButtonStates.None)
			{
				bs =  btns.GetButtonStates();
			}
			do
			{				
				System.Threading.Thread.Sleep(50);
				bs = btns.GetButtonStates();
			} while (bs == Buttons.ButtonStates.None && !close);
			return bs;
		}
		
		public void Close ()
		{
			close = true;
			Wait ();	
		}
		
		public void Wait()
		{
			inputThreadDone.WaitOne();
		}
		
		public void Show()
		{
			inputThreadDone = new ManualResetEvent(false);
			new System.Threading.Thread(handleInputThread).Start();
		}
		
		private void handleInputThread ()
		{
			close = false;
			while (!close) {
				Redraw ();
				switch (WaitForInput ()) {
				case Buttons.ButtonStates.Down: 
					if (dialogItem.DownAction()) 
					{
						close = true;
					}
					break;
				case Buttons.ButtonStates.Up:
					if (dialogItem.UpAction ()) 
					{
						close = true;
					}
					break;
				case Buttons.ButtonStates.Escape:
					if (dialogItem.Escape()) 
					{
						close = true;
					}
					break;
				case Buttons.ButtonStates.Enter:
					if (dialogItem.EnterAction ()) 
					{
						close = true;
					}
					break;
				case Buttons.ButtonStates.Left:
					if (dialogItem.LeftAction()) 
					{
						close = true;
					}
					break;
				case Buttons.ButtonStates.Right:
					if (dialogItem.RightAction()) 
					{
						close = true;
					}
					break;
				}
			}
			inputThreadDone.Set();
		
		}
		
	}
}

