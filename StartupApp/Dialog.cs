using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.IO;
using MonoBrickFirmware.Graphics;

namespace StartupApp
{
	public class InfoDialog: Dialog{
		private string message;
		public InfoDialog(Font f, Lcd lcd, Buttons btns, string message, string title = "Information"):base(f,lcd,btns,title){
			this.message = message;
		}
		
		public void UpdateMessage(string message){
			this.message  = message;
			Draw();
		}
		
		/// <summary>
		/// Show the dialog - this does not block only draws the dialog
		/// </summary>
		public override void Show ()
		{
			Draw();
			//Don't listen for button events
		}
		
		protected override void OnDrawContent (Font f, Rectangle rect)
		{
			int width = rect.P2.X - rect.P1.X;
			int textRectRatio = font.TextSize (message).X / (width);
			if (textRectRatio == 0) {
				lcd.WriteTextBox (f, rect, message, true, Lcd.Alignment.Center);
					
			} 
			else 
			{
				Rectangle top = rect + new Point (0, (int)-f.maxHeight);
				Rectangle buttom = rect + new Point (0, (int)f.maxHeight);
				Rectangle[] rects = { top, rect, buttom };
				string[] words = message.Split (' ');
				int rectIndex = 0;
				string s = "";
				for (int i = 0; i < words.Length; i++) {
					if (f.TextSize (s + " " + words [i]).X < width) {
						if (s == "") {
							s = words [i]; 
						} else {
							s = s + " " + words [i];
						}
					} else {
						lcd.WriteTextBox (f, rects [rectIndex], s, true, Lcd.Alignment.Center);
						s = words[i];
						rectIndex++;
						if (rectIndex >= rects.Length)
							break;
					}  			
				
				}
				if (s != "" && rectIndex < rects.Length) 
				{
					lcd.WriteTextBox (f, rects [rectIndex], s, true, Lcd.Alignment.Center);
				}
			}
		}
	}
	
	public abstract class Dialog
	{
		protected Lcd lcd;
		protected Font font;
		protected string title;
		private int titleSize;
		private int itemSize;
		private int itemHeight;
		private Buttons btns;
		private const float dialogHeightPct = 0.70f;
		private const float dialogWidthPct = 0.90f;
		private const int dialogEdge = 5;
		private Rectangle dialogWindowOuther; 
		private Rectangle dialogWindowInner;
		private Rectangle itemWindow;
		private Rectangle titleRect;
		private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
		private CancellationToken token;
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
			token = cancelTokenSource.Token;
		}
		
		protected void Cancel()
		{
			cancelTokenSource.Cancel();	
		}
		
		public virtual void Show()
		{
			bool exit = false;
			while (!exit && !token.IsCancellationRequested) {
				Draw ();
				switch (btns.GetKeypress(token)) {
					case Buttons.ButtonStates.Down: 
						if (OnDownAction()) 
						{
							exit = true;
						}
						break;
					case Buttons.ButtonStates.Up:
						if (OnUpAction ()) 
						{
							exit = true;
						}
						break;
					case Buttons.ButtonStates.Escape:
						if (OnEscape()) 
						{
							exit = true;
						}
						break;
					case Buttons.ButtonStates.Enter:
						if (OnEnterAction ()) 
						{
							exit = true;
						}
						break;
					case Buttons.ButtonStates.Left:
						if (OnLeftAction()) 
						{
							exit = true;
						}
						break;
					case Buttons.ButtonStates.Right:
						if (OnRightAction()) 
						{
							exit = true;
						}
						break;
				}
			}
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
		
		protected  abstract void OnDrawContent (Font f, Rectangle rect);
		
		protected virtual void Draw ()
		{
			lcd.DrawBox(dialogWindowOuther, true);
			lcd.DrawBox(dialogWindowInner, false);
			OnDrawContent(this.font,itemWindow);
			lcd.WriteTextBox(font,titleRect,title, false,Lcd.Alignment.Center); 
			lcd.Update();
		}
	}
}

