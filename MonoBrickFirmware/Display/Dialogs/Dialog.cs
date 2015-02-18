using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Display.Dialogs
{
    public abstract class Dialog
	{
		
		protected Font font;
		protected Rectangle outherWindow; 
		protected Rectangle innerWindow;
		protected List<Rectangle> lines;
        
				
		private string title;
        
		private Rectangle titleRect;
		private Point bottomLineCenter;
        
        private int titleSize;
		private int dialogWidth;
		private int dialogHeight;

		private const int dialogEdge = 5;
		private const int buttonEdge = 2;
		private const int buttonTextOffset = 2;
		private const int boxMiddleOffset = 8;

		public Action OnShow = delegate {};
		public Action OnExit = delegate {};
		
		public Dialog (Font f, string title, int width = 160, int height = 90, int topOffset = 0)
		{
			dialogWidth = width;
			dialogHeight = height;
			this.font = f;
			this.title = title;
			int xEdge = (Lcd.Width - dialogWidth)/2;
			int yEdge = (Lcd.Height - dialogHeight)/2;
			Point startPoint1 = new Point (xEdge, yEdge);
			Point startPoint2 = new Point (xEdge + dialogWidth, yEdge + dialogHeight);
			this.titleSize = font.TextSize (this.title).X + (int)f.maxWidth;
			outherWindow = new Rectangle (startPoint1, startPoint2);
			innerWindow = new Rectangle (new Point (startPoint1.X + dialogEdge, startPoint1.Y + dialogEdge), new Point (startPoint2.X - dialogEdge, startPoint2.Y - dialogEdge));
			titleRect = new Rectangle (new Point ((int)(Lcd.Width / 2 - titleSize / 2), (int)(startPoint1.Y - (font.maxHeight / 2))), new Point ((int)(Lcd.Width / 2 + titleSize / 2), (int)(startPoint1.Y + (font.maxHeight / 2))));
			int top = innerWindow.P1.Y + (int)( f.maxHeight/2) + topOffset;
			int middel = innerWindow.P1.Y  + ((innerWindow.P2.Y - innerWindow.P1.Y) / 2) - (int)(f.maxHeight)/2;
			int count = 0;
			while (middel > top) {
				middel = middel-(int)f.maxHeight;
				count ++;
			}
			int numberOfLines = count*2+1;
			Point start1 = new Point (innerWindow.P1.X, topOffset+  innerWindow.P1.Y  + ((innerWindow.P2.Y - innerWindow.P1.Y) / 2) - (int)f.maxHeight/2 - count*((int)f.maxHeight) );
			Point start2 = new Point (innerWindow.P2.X, start1.Y + (int)f.maxHeight);
			lines = new List<Rectangle>();
			for(int i = 0; i < numberOfLines; i++){
				lines.Add(new Rectangle(new Point(start1.X, start1.Y+(i*(int)f.maxHeight)),new Point(start2.X,start2.Y+(i*(int)f.maxHeight))));	
            }
			bottomLineCenter = new Point(innerWindow.P1.X + ((innerWindow.P2.X-innerWindow.P1.X)/2) , outherWindow.P2.Y - dialogEdge/2);
			OnShow += delegate {Lcd.Instance.SaveScreen();};
			OnExit += delegate {Lcd.Instance.LoadScreen();};	
		}

		public bool Show ()
		{
			return Show(CancellationToken.None);
		}



		public virtual bool Show(CancellationToken token)
		{
			bool exit = false;
			OnShow();
			while (!exit && !token.IsCancellationRequested) {
				Draw ();
				switch (Buttons.Instance.GetKeypress(token)) {
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
			OnExit();
			return true;
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

		protected void WriteTextOnLine (string text, int lineIndex, bool color = true, Lcd.Alignment alignment = Lcd.Alignment.Center)
		{
			Font f = font;
			string s = text;
			int lineWidth = lines [lineIndex].P2.X - lines [lineIndex].P1.X;
			if (f.TextSize (text.Remove(text.Length-1)).X > lineWidth)
			{
				f = Font.SmallFont;
				while (f.TextSize (s).X > lineWidth) 
				{
					s = s.Remove(s.Length-1);
				}
			}
			Lcd.Instance.WriteTextBox(f, lines[lineIndex], s, color, alignment); 
		}
		
		protected void DrawCenterButton (string text, bool color)
		{
			DrawCenterButton(text,color,0);
		}
		
		protected void DrawCenterButton (string text, bool color, int textSize)
		{
			if (textSize == 0) 
			{
				textSize = font.TextSize(text).X;	
			}
			textSize+= buttonTextOffset;
			Point buttonP1 = bottomLineCenter + new Point((int)-textSize/2,(int)-font.maxHeight/2);
			Point buttonP2 = bottomLineCenter + new Point((int)textSize/2,(int)font.maxHeight/2);
			
			Point buttonP1Outer = buttonP1 + new Point(-buttonEdge,-buttonEdge);
			Point buttonp2Outer = buttonP2 + new Point(buttonEdge,buttonEdge);
			
			Rectangle buttonRect = new Rectangle(buttonP1, buttonP2);
			Rectangle buttonRectEdge = new Rectangle(buttonP1Outer, buttonp2Outer);
			
			Lcd.Instance.DrawRectangle(buttonRectEdge,true, true);
			Lcd.Instance.WriteTextBox(font,buttonRect,text, color, Lcd.Alignment.Center);		
		}
		
		protected void DrawLeftButton (string text, bool color)
		{
			DrawLeftButton (text, color, 0);
		}
		
		protected void DrawLeftButton (string text, bool color, int textSize)
		{
			
			if (textSize == 0) 
			{
				textSize = font.TextSize(text).X;	
			}
			textSize+= buttonTextOffset;
			Point left1 = bottomLineCenter + new Point(-boxMiddleOffset - (int)textSize,(int)-font.maxHeight/2);
			Point left2 = bottomLineCenter + new Point(-boxMiddleOffset,(int)font.maxHeight/2);
			Point leftOuter1 = left1 + new Point(-buttonEdge,-buttonEdge);
			Point leftOuter2 = left2 + new Point(buttonEdge,buttonEdge);
			
			Rectangle leftRect = new Rectangle(left1, left2);
			Rectangle leftOuterRect = new Rectangle(leftOuter1, leftOuter2);
			
			Lcd.Instance.DrawRectangle(leftOuterRect,true, true);
			Lcd.Instance.WriteTextBox(font, leftRect, text, color, Lcd.Alignment.Center);
		
		}
		
		protected void DrawRightButton (string text, bool color)
		{
			DrawRightButton (text, color, 0);
		}
		
		protected void DrawRightButton (string text, bool color, int textSize)
		{
			if (textSize == 0) 
			{
				textSize = font.TextSize(text).X;	
			}
			textSize+= buttonTextOffset;
			Point right1 = bottomLineCenter + new Point(boxMiddleOffset,(int)-font.maxHeight/2);
			Point right2 = bottomLineCenter + new Point(boxMiddleOffset + (int)textSize,(int)font.maxHeight/2);
			Point rightOuter1 = right1 + new Point(-buttonEdge,-buttonEdge);
			Point rightOuter2 = right2 + new Point(buttonEdge,buttonEdge);
			
			
			Rectangle rightRect = new Rectangle(right1, right2);
			Rectangle rightOuterRect = new Rectangle(rightOuter1, rightOuter2);
			
			Lcd.Instance.DrawRectangle(rightOuterRect, true, true);
			
			Lcd.Instance.WriteTextBox(font, rightRect, text, color, Lcd.Alignment.Center);
		
		}

		protected void WriteTextOnDialog (string text)
		{
			int width = lines [0].P2.X - lines [0].P1.X;
			int textRectRatio = font.TextSize (text).X / (width);
			if (textRectRatio == 0) {
				int middle = (lines.Count / 2);
				Lcd.Instance.WriteTextBox (font, lines [middle], text, true, Lcd.Alignment.Center);
			} 
			else 
			{
				string[] words = text.Split (' ');
				int rectIndex = 0;
				string s = "";
				for (int i = 0; i < words.Length; i++) {
					if (font.TextSize (s + " " + words [i]).X < width) {
						if (s == "") {
							s = words [i]; 
						} else {
							s = s + " " + words [i];
						}
					} else {
						Lcd.Instance.WriteTextBox (font, lines [rectIndex], s, true, Lcd.Alignment.Center);
						s = words [i];
						rectIndex++;
						if (rectIndex >= lines.Count)
							break;
					}  			
				
				}
				if (s != "" && rectIndex < lines.Count) {
					Lcd.Instance.WriteTextBox (font, lines [rectIndex], s, true, Lcd.Alignment.Center);
				}
			}
		}
		
		protected  abstract void OnDrawContent ();
		
		protected void ClearContent ()
		{
			Lcd.Instance.LoadScreen();
			Lcd.Instance.DrawRectangle(outherWindow, true, true);
			Lcd.Instance.DrawRectangle(innerWindow, false, true);
			Lcd.Instance.WriteTextBox(font,titleRect,title, false,Lcd.Alignment.Center); 
		}
		
		protected virtual void Draw ()
		{
			Lcd.Instance.DrawRectangle(outherWindow, true, true);
			Lcd.Instance.DrawRectangle(innerWindow, false, true);
			OnDrawContent();
			Lcd.Instance.WriteTextBox(font,titleRect,title, false,Lcd.Alignment.Center); 
			Lcd.Instance.Update();
			 
		}
	}
}

