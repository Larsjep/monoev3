using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Buttons;

namespace StartupApp
{
    public class CharacterDialog : Dialog
    {

        public CharacterDialog(Font f, Lcd lcd, Buttons btns, string title)
            : base(f, lcd, btns, title) 
        {
        
        } 
        
        protected override void OnDrawContent(){
        
        
        }

    }
    
    public class SelectDialog<SelectionType> : Dialog { 
        private SelectionType[] options;
        private int scrollPos;
        int cursorPos;
		bool allowEsc;
		
        private const int notificationEdge = 2;
		public SelectDialog (Font f, Lcd lcd, Buttons btns, SelectionType[] options, string title, bool allowEsc) : base (f, lcd, btns, title)
		{
			this.options = options;
			cursorPos = 0;
			scrollPos = 0;
			this.allowEsc = allowEsc;
			EscPressed = false;
        }

        protected override void OnDrawContent ()
		{
			for (int i = 0; i != lines.Count; ++i) {
				if (i + scrollPos >= options.Length)
					break;
				lcd.WriteTextBox (font, lines [i], options [i + scrollPos].ToString (), i != cursorPos, Lcd.Alignment.Center); 	
			}
        }

        protected override bool OnUpAction ()
		{
			if (cursorPos + scrollPos > 0) {
				if (cursorPos > 0)
					cursorPos--;
				 else 
					scrollPos--;
			} 
			return false;
        }

        protected override bool OnDownAction ()
		{
			if (scrollPos + cursorPos < options.Length - 1) {
				if (cursorPos < lines.Count - 1)
					cursorPos++;
				else
					scrollPos++;
			} 
           	return false;
        }
        
        protected override bool OnEnterAction ()
		{
			return true;
		}
		
		protected override bool OnEscape ()
		{
			if (allowEsc) {
				EscPressed = true;
				return true;
			}
			return false;
		}
        
		public SelectionType GetSelection()
		{
			if(EscPressed)
				return default(SelectionType);
			else
				return options[cursorPos+scrollPos];
		}
		
		public bool EscPressed{get; private set;}
        
    }
    
    
    
    
    public class QuestionDialog : Dialog{
		private string negativeText;
		private string positiveText;
		private string question;
		private const int boxEdge = 2;
		private const int boxMiddleOffset = 8;
		private const int textOffset = 2;
		public QuestionDialog (Font f, Lcd lcd, Buttons btns, string question, string title, string positiveText="Yes", string negativeText="No", bool isPositiveSelected = true) : base (f, lcd, btns, title)
		{
			this.negativeText = negativeText;
			this.positiveText = positiveText;
			this.question = question;
			this.IsPositiveSelected = isPositiveSelected;
		}
		
		protected override bool OnLeftAction ()
		{
			if(!IsPositiveSelected)
				IsPositiveSelected = true;
			return false;
		}
		
		protected override bool OnRightAction ()
		{
			if(IsPositiveSelected)
				IsPositiveSelected = false;
			return false;
		}
		
		protected override bool OnEnterAction ()
		{
			return true;
		}
		
		protected override void OnDrawContent ()
		{
			WriteTextOnDialog(question);
			int textSize = 0;
			if (font.TextSize (positiveText).X > font.TextSize (negativeText).X) 
			{
				textSize = font.TextSize(positiveText).X;
			} 
			else 
			{
				textSize = font.TextSize(negativeText).X;
			}
			textSize+=textOffset;
			
			Point positive1 = bottomLineCenter + new Point(-boxMiddleOffset - (int)textSize,(int)-font.maxHeight/2);
			Point positive2 = bottomLineCenter + new Point(-boxMiddleOffset,(int)font.maxHeight/2);
			Point positiveOuter1 = positive1 + new Point(-boxEdge,-boxEdge);
			Point positiveOuter2 = positive2 + new Point(boxEdge,boxEdge);
			
			
			Point negative1 = bottomLineCenter + new Point(boxMiddleOffset,(int)-font.maxHeight/2);
			Point negative2 = bottomLineCenter + new Point(boxMiddleOffset + (int)textSize,(int)font.maxHeight/2);
			Point negativeOuter1 = negative1 + new Point(-boxEdge,-boxEdge);
			Point negativeOuter2 = negative2 + new Point(boxEdge,boxEdge);
			
			
			Rectangle positiveRect = new Rectangle(positive1, positive2);
			Rectangle negativeRect = new Rectangle(negative1, negative2);
			Rectangle positiveOuterRect = new Rectangle(positiveOuter1, positiveOuter2);
			Rectangle negativeOuterRect = new Rectangle(negativeOuter1, negativeOuter2);
			
			lcd.DrawBox(positiveOuterRect,true);
			lcd.DrawBox(negativeOuterRect, true);
			
			lcd.WriteTextBox(font, positiveRect, positiveText, !IsPositiveSelected, Lcd.Alignment.Center);
			lcd.WriteTextBox(font, negativeRect, negativeText, IsPositiveSelected, Lcd.Alignment.Center);
		}
		
		public bool IsPositiveSelected{get; private set;}
	
	}
	
	
	
	public class InfoDialog: Dialog{
		private string message;
		private bool waitForOk;
		private const string okString = "OK";
		private const int boxEdge = 2;
		private const int textOffset = 2;
		public InfoDialog(Font f, Lcd lcd, Buttons btns, string message, bool waitForOk, string title = "Information"):base(f,lcd,btns,title){
			this.message = message;
			this.waitForOk = waitForOk;
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
			if (waitForOk) {
				base.Show ();
			} 
			else {
				Draw ();
				//Don't listen for button events
			}
		}
		
		protected override bool OnEnterAction ()
		{
			return true;//exit
		}
		
		
		protected override void OnDrawContent ()
		{
			WriteTextOnDialog(message);
			if (waitForOk) {
				int textSize = font.TextSize(okString).X;
				textSize+= textOffset;
				Point okp1 = bottomLineCenter + new Point((int)-textSize/2,(int)-font.maxHeight/2);
				Point okp2 = bottomLineCenter + new Point((int)textSize/2,(int)font.maxHeight/2);
				
				Point okp1Outer = okp1 + new Point(-boxEdge,-boxEdge);
				Point okp2Outer = okp2 + new Point(boxEdge,boxEdge);
				
				Rectangle okRect = new Rectangle(okp1, okp2);
				Rectangle okRectEdge = new Rectangle(okp1Outer, okp2Outer);
				
				lcd.DrawBox(okRectEdge,true);
				lcd.WriteTextBox(font,okRect,okString,false,Lcd.Alignment.Center);	
			}
		}
	}
	
	public abstract class Dialog
	{
		protected Lcd lcd;
		protected Font font;
		protected string title;
        protected List<Rectangle> lines;
        protected Rectangle dialogWindowOuther; 
		protected Rectangle dialogWindowInner;
		protected Rectangle titleRect;
		protected Point bottomLineCenter;
        
        private int titleSize;
		private Buttons btns;
		private const float dialogHeightPct = 0.70f;
		private const float dialogWidthPct = 0.90f;
		private const int dialogEdge = 5;
		private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
		private CancellationToken token;
		public Dialog (Font f, Lcd lcd, Buttons btns, string title)
		{
			this.font = f;
			this.lcd = lcd;
			this.title = title;
			this.btns = btns;
			int xEdge = (int)((float)(Lcd.Width) * (1.0 - dialogWidthPct) / 2);
			int yEdge = (int)((float)(Lcd.Height) * (1.0 - dialogHeightPct) / 2);
			int ySize = (int)((float)(Lcd.Height) * dialogHeightPct);
			int xSize = (int)((float)(Lcd.Width) * dialogWidthPct);
			Point startPoint1 = new Point (xEdge, yEdge);
			Point startPoint2 = new Point (xEdge + xSize, yEdge + ySize);
			this.titleSize = font.TextSize (this.title).X + (int)f.maxWidth;
			dialogWindowOuther = new Rectangle (startPoint1, startPoint2);
			dialogWindowInner = new Rectangle (new Point (startPoint1.X + dialogEdge, startPoint1.Y + dialogEdge), new Point (startPoint2.X - dialogEdge, startPoint2.Y - dialogEdge));
			titleRect = new Rectangle (new Point ((int)(Lcd.Width / 2 - titleSize / 2), (int)(startPoint1.Y - (font.maxHeight / 2))), new Point ((int)(Lcd.Width / 2 + titleSize / 2), (int)(startPoint1.Y + (font.maxHeight / 2))));
			token = cancelTokenSource.Token;
			
						
			int top = dialogWindowInner.P1.Y + (int)( f.maxHeight/2);
			int middel = dialogWindowInner.P1.Y  + ((dialogWindowInner.P2.Y - dialogWindowInner.P1.Y) / 2) - (int)(f.maxHeight)/2;
			int count = 0;
			while (middel > top) {
				middel = middel-(int)f.maxHeight;
				count ++;
			}
			int numberOfLines = count*2+1;
			Point start1 = new Point (dialogWindowInner.P1.X, dialogWindowInner.P1.Y  + ((dialogWindowInner.P2.Y - dialogWindowInner.P1.Y) / 2) - (int)f.maxHeight/2 - count*((int)f.maxHeight) );
			Point start2 = new Point (dialogWindowInner.P2.X, start1.Y + (int)f.maxHeight);
			lines = new List<Rectangle>();
			for(int i = 0; i < numberOfLines; i++){
				lines.Add(new Rectangle(new Point(start1.X, start1.Y+(i*(int)f.maxHeight)),new Point(start2.X,start2.Y+(i*(int)f.maxHeight))));	
            }
			bottomLineCenter = new Point(dialogWindowInner.P1.X + ((dialogWindowInner.P2.X-dialogWindowInner.P1.X)/2) , dialogWindowOuther.P2.Y - dialogEdge/2);
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
		
		protected void WriteTextOnDialog (string text)
		{
			int width = lines [0].P2.X - lines [0].P1.X;
			int textRectRatio = font.TextSize (text).X / (width);
			if (textRectRatio == 0) {
				int middle = (lines.Count / 2);
				lcd.WriteTextBox (font, lines [middle], text, true, Lcd.Alignment.Center);
			} else {
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
						lcd.WriteTextBox (font, lines [rectIndex], s, true, Lcd.Alignment.Center);
						s = words [i];
						rectIndex++;
						if (rectIndex >= lines.Count)
							break;
					}  			
				
				}
				if (s != "" && rectIndex < lines.Count) {
					lcd.WriteTextBox (font, lines [rectIndex], s, true, Lcd.Alignment.Center);
				}
			}
		
		
		
		}
		
		
		protected  abstract void OnDrawContent ();
		
		protected virtual void Draw ()
		{
			lcd.DrawBox(dialogWindowOuther, true);
			lcd.DrawBox(dialogWindowInner, false);
			OnDrawContent();
			lcd.WriteTextBox(font,titleRect,title, false,Lcd.Alignment.Center); 
			lcd.Update();
		}
	}
}

