using System;

namespace MonoBrickFirmware.Display.Dialogs.UserInput
{
	
	public enum ExitType {Left, Center, Right};

	public struct Position
	{
		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}
		public int X{ get; set;}
		public int Y{ get; set;}
	}

	public interface IButton
	{
		string Text{ get; set; }
		bool Selected{ get; set;}
		bool Disabled{ get; set;}
		string Id{ get;}
		ExitType ExitType{ get;}
		void Draw ();
		Position Position{get;}
		Position Size{get;}
	}


	public class Button : IButton
	{
		private string text; 
		protected const int yTop = 3;
		protected const int charecterOffset = 4;
		protected const int characterSize = 14; 
		protected const int characterEdge = 1;
		protected Rectangle outherBox;
		protected Rectangle innerBox;
		protected Point textCenterPoint;
		public Button(string text, Position position, Position size, Rectangle container, string id, bool disable = false, ExitType exitType = ExitType.Center)
		{
			Text = text;
			this.Id = id;
			this.Position = position;
			this.Size = size;
			this.Disabled = disable;
			this.Selected = false;
			this.ExitType = exitType;
			ApplyContainer (container);
		}
		public virtual string Text { get{ return text; } set{ text = value; CalculateTextCenterPoint (); } }
		public bool Selected{ get; set;}
		public bool Disabled{ get; set;}
		public string Id{ get; protected set;}
		public ExitType ExitType{ get; protected set;}
		public virtual void Draw()
		{
			if (Disabled)
			{
				Lcd.DrawRectangle(outherBox, false, false);
				Lcd.DrawRectangle (innerBox, Selected, true);
				return;			
			}
			Lcd.DrawRectangle(outherBox, true, true);
			Lcd.DrawRectangle (innerBox, Selected, true);
			Lcd.WriteText (Font.MediumFont, textCenterPoint, Text, !Selected);
		}

		private void CalculateTextCenterPoint()
		{
			int centerX = (outherBox.P2.X - outherBox.P1.X)/2;
			int textSizeX = (Font.MediumFont.TextSize (Text).X)/2;
			int textSizeY = (Font.MediumFont.TextSize (Text).Y)/2;
			int centerY = (outherBox.P2.Y - outherBox.P1.Y)/2;
			textCenterPoint = new Point (innerBox.P1.X + centerX-textSizeX, innerBox.P1.Y + centerY-textSizeY-characterEdge);
		}

		public Position Position{ get; protected set;}
		public Position Size{ get; protected set;}

		private void ApplyContainer(Rectangle container)
		{
			int characterHeight = (int)Font.MediumFont.maxHeight / 2;
			Point buttonSpace = new Point(1,0);
			Point buttonInnerPoint = new Point(characterSize, characterSize); 
			Point buttonOutherPoint = new Point(buttonInnerPoint.X + 2* characterEdge, buttonInnerPoint.Y + 2* characterEdge);
			for (int yPos = 0; yPos < Position.Y +1; yPos++) 
			{
				Point start = new Point (container.P1.X, container.P1.Y + characterHeight - yTop + yPos * buttonOutherPoint.Y);  
				outherBox = new Rectangle (start, start + buttonOutherPoint);
				for(int xPos = 0; xPos < Position.X; xPos++)
				{
					outherBox = outherBox + new Point (buttonOutherPoint.X, 0) + buttonSpace;
				}
			}
			Point reSize = new Point (outherBox.P2.X + ((buttonOutherPoint.X + buttonSpace.X) * (Size.X - 1)), outherBox.P2.Y + ((buttonOutherPoint.Y + buttonSpace.Y) * (Size.Y - 1))); 
			outherBox = new Rectangle (outherBox.P1, reSize);
			innerBox = new Rectangle (new Point (outherBox.P1.X + characterEdge, outherBox.P1.Y + characterEdge), new Point (outherBox.P2.X - characterEdge, outherBox.P2.Y - characterEdge));
			CalculateTextCenterPoint (); 
		}
	}

	public class Letter : Button
	{
		public Letter (string text, Position position, Rectangle container) : base(text, position, new Position(1, 1), container, "Letter")
		{

		}
		public override string Text
		{
			get 
			{
				return base.Text;
			}
			set 
			{
				base.Text = value;
				Disabled = value == "";
			}
		}
	} 

	public class Select : Button
	{
		private Lcd.ArrowOrientation arrowOrientation;
		private Rectangle arrowRect;
		public Select(Position position, Rectangle container, bool next) : base("", position, new Position(1,1), container, next ? "SelectNext" : "SelectPrev")
		{
			arrowOrientation = next ? Lcd.ArrowOrientation.Right : Lcd.ArrowOrientation.Left;
			arrowRect = new Rectangle (new Point (outherBox.P1.X + 3 * characterEdge, outherBox.P1.Y + 3 * characterEdge), new Point (outherBox.P2.X - 3 * characterEdge, outherBox.P2.Y - 3 * characterEdge));
		}

		public override void Draw ()
		{
			if (Disabled)
			{
				Lcd.DrawRectangle(outherBox, true, true);
				Lcd.DrawRectangle (innerBox, Selected, true);
				Lcd.DrawLine (outherBox.P1, outherBox.P2, !Selected);
				Lcd.DrawLine (new Point(outherBox.P1.X,outherBox.P2.Y ), new Point(outherBox.P2.X,outherBox.P1.Y), !Selected);
				return;			
			}
			Lcd.DrawRectangle(outherBox, true, true);
			Lcd.DrawRectangle (innerBox, Selected, true);
			Lcd.DrawArrow (arrowRect, arrowOrientation, !Selected);
		}
	}

	public class Shift : Button
	{
		private Rectangle arrowRect;
		private bool small;
		Point lineStart;
		int lineLength;
		Point textDisplacement;
		public Shift(Position position, Rectangle container, ExitType exitType, bool small = true) : base("Shift", position, small ? new Position(3,1) : new Position(2,2), container, "Shift", false, exitType)
		{
			this.small = small;
			arrowRect = new Rectangle (new Point (outherBox.P1.X + 3 * characterEdge, outherBox.P1.Y + 2 * characterEdge), new Point (outherBox.P1.X + 14 * characterEdge, outherBox.P2.Y - 7 * characterEdge));
			if (!small)
			{
				arrowRect = new Rectangle (new Point (arrowRect.P1.X, arrowRect.P1.Y), new Point (arrowRect.P2.X, arrowRect.P1.Y + (arrowRect.P2.Y - arrowRect.P1.Y)/2 - 2 * characterEdge));
				arrowRect = arrowRect + new Point (7, 0);
			}
			lineStart = new Point(arrowRect.P1.X + ((arrowRect.P2.X - arrowRect.P1.X) / 2), arrowRect.P2.Y);
			lineLength = (innerBox.P2.Y - lineStart.Y)  - 2 * characterEdge;
			if (small) 
			{
				lineLength = (innerBox.P2.Y - lineStart.Y)  - 2 * characterEdge;
				textDisplacement = new Point (10, 6);
			} 
			else
			{
				lineLength = (innerBox.P2.Y - lineStart.Y)/3;
				textDisplacement = new Point (5, 14);
			}
		}

		public override void Draw ()
		{
			Lcd.DrawRectangle(outherBox, true, true);
			Lcd.DrawRectangle (innerBox, Selected, true);
			Lcd.WriteText (Font.SmallFont, textCenterPoint + textDisplacement, Text, !Selected);
			Lcd.DrawArrow (arrowRect, Lcd.ArrowOrientation.Up, !Selected);
			Lcd.DrawVLine(lineStart, lineLength, !Selected);
			Lcd.DrawVLine(lineStart + new Point(-1,0) , lineLength, !Selected); 			
			Lcd.DrawVLine(lineStart + new Point(+1,0) , lineLength, !Selected); 			

		}

	}

	public class Ok : Button
	{
		public Ok(Position position, Rectangle container, ExitType exitType) : this(position, container, exitType, new Position(2,1))
		{

		}
		public Ok(Position position, Rectangle container, ExitType exitType, Position size) : base("Ok", position, size, container, "Ok", false, exitType)
		{
			
		}
	}

	public class Enter : Button
	{
		private Rectangle arrowRect;
		public Enter(Position position, Rectangle container, ExitType exitType) : base("", position, new Position(2,1), container, "Enter", false, exitType)
		{
			arrowRect = new Rectangle (new Point (outherBox.P1.X + characterEdge, outherBox.P1.Y + 4 * characterEdge), new Point (outherBox.P2.X - ((outherBox.P2.X - outherBox.P1.X)/2) - 3* characterEdge, outherBox.P2.Y - 3 * characterEdge));
		}
		public override void Draw ()
		{
			Lcd.DrawRectangle(outherBox, true, true);
			Lcd.DrawRectangle (innerBox, Selected, true);
			Lcd.DrawArrow (arrowRect, Lcd.ArrowOrientation.Left, !Selected);
			Lcd.DrawLine (new Point(arrowRect.P2.X, outherBox.P1.Y + ((outherBox.P2.Y - outherBox.P1.Y)/2)), new Point(outherBox.P2.X - 8 * characterEdge, outherBox.P1.Y +((outherBox.P2.Y - outherBox.P1.Y)/2) ), !Selected);
			Lcd.DrawLine (new Point(outherBox.P2.X - 8 * characterEdge, outherBox.P1.Y + ((outherBox.P2.Y - outherBox.P1.Y)/2)), new Point(outherBox.P2.X - 8 * characterEdge, outherBox.P1.Y + 4* characterEdge), !Selected);
		}
	}

	public class Space : Button
	{
		public Space(Position position, Rectangle container, ExitType exitType, int length = 5) : base("space", position, new Position(length,1), container, "Space", false, exitType)
		{

		}
	}


}

