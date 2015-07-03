using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using System.Threading;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithNumericInput : IChildItem
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
		private CancellationTokenSource cancelSource = new CancellationTokenSource();

		public Action<int> OnValueChanged = delegate {};
		public ItemWithNumericInput (string text, int startValue, int min = int.MinValue, int max= int.MaxValue){
			this.text = text;
			this.Value = startValue;
			this.min = min;
			this.max = max;
		}

		public IParentItem Parent { get; set;}

		public void OnLeftPressed ()
		{
			int counter = 0;
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
				this.OnDrawTitle(font,rect,false);
				Lcd.Update();
				System.Threading.Thread.Sleep(holdSleepTime);
			}while (Buttons.GetStates()== Buttons.ButtonStates.Left && !cancelSource.Token.IsCancellationRequested);
			OnValueChanged(Value);
		}
		
		public void OnRightPressed(){
			int counter = 0;
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
				this.OnDrawTitle(font,rect,false);
				Lcd.Update();
				System.Threading.Thread.Sleep(holdSleepTime);
			}while (Buttons.GetStates()== Buttons.ButtonStates.Right && !cancelSource.Token.IsCancellationRequested);
			OnValueChanged(Value);
		}

		public void OnDrawTitle (Font f, Rectangle r, bool color)
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
			
			Lcd.WriteTextBox (f, textRect, text, color, Lcd.Alignment.Left);
			Lcd.WriteTextBox (f, numericRect, valueAsString, color, Lcd.Alignment.Right);
			Lcd.DrawArrow(leftArrowRect, Lcd.ArrowOrientation.Left, color);
			Lcd.DrawArrow(rightArrowRect, Lcd.ArrowOrientation.Right, color);
		}

		public void OnEnterPressed()
		{
		
		}

		public void OnUpPressed ()
		{
			
		}

		public void OnDownPressed ()
		{
			
		}

		public void OnEscPressed ()
		{
			
		}

		public void OnDrawContent ()
		{
			
		}

		public void OnHideContent ()
		{
			cancelSource.Cancel ();	
		}

		public int Value{get;private set;}
	}
}

