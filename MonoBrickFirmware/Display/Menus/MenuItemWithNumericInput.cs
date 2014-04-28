using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Display.Menus
{
	public class MenuItemWithNumericInput : IMenuItem
	{
		private string text;
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
		public Action<int> OnValueChanged = delegate {};
		public MenuItemWithNumericInput (string text, int startValue, int min = int.MinValue, int max= int.MaxValue){
			this.text = text;
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
					Lcd.Instance.Update();
					System.Threading.Thread.Sleep(holdSleepTime);
				}while (btns.GetButtonStates()== Buttons.ButtonStates.Left);
			}
			OnValueChanged(Value);
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
					Lcd.Instance.Update();
					System.Threading.Thread.Sleep(holdSleepTime);
				}while (btns.GetButtonStates()== Buttons.ButtonStates.Right);
			}
			OnValueChanged(Value);
			return false;
		}
		public void Draw (Font f, Rectangle r, bool color)
		{
			font = f;
			rect = r;
			
			int arrowWidth = (int)f.maxWidth / 4;
			
			string valueAsString = " " + Value.ToString () + " ";
			Point p = f.TextSize (valueAsString);
			Rectangle numericRect = new Rectangle ( new Point( Lcd.Width - p.X, r.P1.Y),r.P2);
			Rectangle textRect = new Rectangle (new Point (r.P1.X, r.P1.Y), new Point (r.P2.X - (p.X), r.P2.Y));
			Rectangle leftArrowRect = new Rectangle(new Point(numericRect.P1.X, numericRect.P1.Y+arrowEdge), new Point(numericRect.P1.X+ arrowWidth, numericRect.P2.Y-arrowEdge));
			Rectangle rightArrowRect = new Rectangle( new Point(numericRect.P2.X-(arrowWidth + rightArrowOffset), numericRect.P1.Y+arrowEdge) , new Point(numericRect.P2.X-rightArrowOffset,numericRect.P2.Y-arrowEdge));
			
			Lcd.Instance.WriteTextBox (f, textRect, text, color, Lcd.Alignment.Left);
			Lcd.Instance.WriteTextBox (f, numericRect, valueAsString, color, Lcd.Alignment.Right);
			Lcd.Instance.DrawArrow(leftArrowRect, Lcd.ArrowOrientation.Left, color);
			Lcd.Instance.DrawArrow(rightArrowRect, Lcd.ArrowOrientation.Right, color);
		}
		public int Value{get;private set;}
	}
}

