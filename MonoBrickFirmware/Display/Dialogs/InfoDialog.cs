using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Display.Dialogs
{
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
		/// Show the dialog - this does not block only draws the dialog if wait for OK is disabled
		/// </summary>
		public override void Show ()
		{
			if (waitForOk) {
				base.Show ();
			} 
			else {
				OnShow();
				Draw ();
				OnExit();
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
}

