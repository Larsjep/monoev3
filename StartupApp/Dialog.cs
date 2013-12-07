using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace StartupApp
{
	
	
	public class InfoDialogWithEscape: Dialog{
		private Func<bool> escapeAction;
		private string message;
		public InfoDialogWithEscape(Font f, Lcd lcd, Buttons btns, string message, Func<bool> escapeAction):base(f,lcd,btns,"Information"){
			this.escapeAction = escapeAction;
			this.message = message;	
		}
		
		public void UpdateMessage(string message){
			this.message  = message;
			base.Draw();
		}
		protected override bool OnEscape ()
		{
			return escapeAction();
		}
		
		protected override void OnDrawContent (Font f, Rectangle rect)
		{
			lcd.WriteTextBox(f,rect,message,true,Lcd.Alignment.Center);
		}
		
		protected override void Draw ()
		{
			//Don't redraw when a button is pressed
		}
		
		public override void Show ()
		{
			base.Show ();
			base.Draw();
		}
	}
	
	public class InfoDialog: Dialog{
		private string message;
		public InfoDialog(Font f, Lcd lcd, Buttons btns, string message):base(f,lcd,btns,"Information"){
			this.message = message;
		}
		
		public void UpdateMessage(string message){
			this.message  = message;
			Draw();
		}
		
		public override void Show ()
		{
			Draw();
			//Don't listen for button events
		}
		
		protected override void OnDrawContent (Font f, Rectangle rect)
		{
			lcd.WriteTextBox(f,rect,message,true,Lcd.Alignment.Center);
		}
	}
	
	public class Dialog
	{
		protected Lcd lcd;
		protected Font font;
		protected string title;
		protected ManualResetEvent inputThreadDone;
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
		public Dialog (Font f, Lcd lcd, Buttons btns, string title)
		{
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
		
		public virtual void Show()
		{
			inputThreadDone = new ManualResetEvent(false);
			new System.Threading.Thread(handleInputThread).Start();
		}
		
		protected virtual bool OnEnterAction ()
		{
			return false;
		}
		
		protected virtual bool OnLeftAction ()
		{
			return false;
		}
		
		protected virtual bool OnRightAction ()
		{
			return false;
		}
		
		protected virtual bool OnUpAction ()
		{
			return false;
		}
		
		protected virtual bool OnDownAction ()
		{
			return false;
		}
		
		protected virtual bool OnEscape(){
			return false;
		}
		
		protected virtual void OnDrawContent (Font f, Rectangle rect)
		{
		
		}
		
		protected virtual void Draw ()
		{
			lcd.DrawBox(dialogWindowOuther, true);
			lcd.DrawBox(dialogWindowInner, false);
			OnDrawContent(this.font,itemWindow);
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
		
		protected void Close ()
		{
			close = true;
			inputThreadDone.WaitOne();	
		}
		
		private void handleInputThread ()
		{
			close = false;
			while (!close) {
				Draw ();
				switch (WaitForInput ()) {
				case Buttons.ButtonStates.Down: 
					if (OnDownAction()) 
					{
						close = true;
					}
					break;
				case Buttons.ButtonStates.Up:
					if (OnUpAction ()) 
					{
						close = true;
					}
					break;
				case Buttons.ButtonStates.Escape:
					if (OnEscape()) 
					{
						close = true;
					}
					break;
				case Buttons.ButtonStates.Enter:
					if (OnEnterAction ()) 
					{
						close = true;
					}
					break;
				case Buttons.ButtonStates.Left:
					if (OnLeftAction()) 
					{
						close = true;
					}
					break;
				case Buttons.ButtonStates.Right:
					if (OnRightAction()) 
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

