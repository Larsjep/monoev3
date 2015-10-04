using System;
using System.Collections.Generic;

namespace MonoBrickFirmware.Display.Dialogs.UserInput
{
	public class Keyboard : ButtonContainer
	{
		private ICharacters characters;
		private bool disableEnter;
		private bool disableSelect;
		private ICharacterSet[] characterSets = new ICharacterSet[]{new Letters(), new NumbersAndSymbols(), new NumbersAndSymbols2()};
		private int characterSetsIdx = 0;
		private bool shiftSet = false;
		private TextButton textButton;
		private bool okWithEsc = false;
		public Keyboard (Rectangle container, bool disableEnter, bool disableSelect) : base(container, 10, 6)
		{
			this.disableSelect = disableSelect;
			this.disableEnter = disableEnter;
			CreateLayout ();
			SelectedButton = buttons [0, 0];
			Characters = characterSets [characterSetsIdx].Characters;
			OnButtonSelected (SelectedButton);
		}
		public event Action OnOk = delegate() {};
		public event Action OnCancel = delegate() {};
		public string Text{ get{ return textButton.Text;}}

		public override void Enter ()
		{
			switch (SelectedButton.Id)
			{
			case "Letter":
				textButton.AddSelectedCharacter ();
				break;
			case "SelectNext":
				OnSelectNext();
				break;
			case "SelectPrev":
				OnSelectPrev ();
				break;
			case "Shift":
				OnShift ();
				break;
			case "Ok":
				OnOk();
				break;
			case "Enter":
				textButton.AddLine ();
				break;
			case "Space":
				textButton.AddSpace ();
				break;
			}
		}

		public override void Esc ()
		{
			if (SelectedButton.Id == "Ok") {
				okWithEsc = true;
				OnCancel ();
			} 
			else 
			{
				textButton.DeleteCharacter ();	
			}	
		}

		public override void Draw ()
		{
			base.Draw ();
			textButton.Draw ();
		}


		protected override void OnButtonSelected(IButton button)
		{
			switch (button.Id) 
			{
				case "Letter":
					textButton.SelectedCharacter = button.Text;
					break;
				case "Space":
					textButton.SelectedCharacter = " ";
					break;
				default:
					textButton.SelectedCharacter = "";
					break;
			}
		}

		protected ICharacters Characters
		{
			get{return characters;}
			set{characters = value; FillCharacters (); }
		}

		protected void CreateLayout()
		{
			CreateSpecialKeys ();
			CreateLetter ();
		}

		private void OnSelectNext()
		{
			characterSetsIdx++;
			if (characterSetsIdx >= characterSets.Length) {
				characterSetsIdx = 0;	
			}
			Characters = characterSets [characterSetsIdx].Characters;
			shiftSet = false;
		}

		private void OnSelectPrev()
		{
			characterSetsIdx--;
			if (characterSetsIdx < 0) {
				characterSetsIdx = characterSets.Length - 1;	
			}
			Characters = characterSets [characterSetsIdx].Characters;
			shiftSet = false;
		}

		private void OnShift()
		{
			if (shiftSet)
			{
				Characters = characterSets [characterSetsIdx].Characters;
				shiftSet = false;
			} 
			else if(characterSets [characterSetsIdx].ShiftCharacters != null)
			{
				Characters = characterSets [characterSetsIdx].ShiftCharacters;
				shiftSet = true;
			}
		}

		private void CreateSpecialKeys()
		{
			if (disableEnter)
			{
				Add(new Ok (new Position(8,2), container, ExitType.Right, new Position(2,2)));
			} 
			else 
			{
				Add(new Enter (new Position(8,2), container, ExitType.Right));
				Add(new Ok (new Position(8,3), container, ExitType.Right));

			}
			if (disableSelect) 
			{
				Add(new Shift (new Position(0,2), container, ExitType.Left, false));
				Add(new Space (new Position(2,3), container, ExitType.Center, 6));
			} 
			else 
			{
				Add(new Select (new Position(0,2), container, false));
				Add(new Select (new Position(1,2), container, true));
				Add(new Shift (new Position(0,3), container, ExitType.Left));
				Add(new Space (new Position(3,3), container, ExitType.Center));
			}
			textButton = new  TextButton ("Something", new Position (0, 4), new Position (10, 2), container);
			Add(textButton); 
		}

		private void CreateLetter()
		{
			for(int yPos = 0; yPos < yMax; yPos++)
			{
				for(int xPos = 0; xPos < xMax; xPos++)
				{
					if (buttons [xPos, yPos] == null)
					{
						Add (new Letter ("", new Position (xPos, yPos), container));
					}
				}			
			}  	
		}

		private void FillCharacters()
		{
			int characterSetIdx = 0;
			for(int yPos = 0; yPos < yMax; yPos++)
			{
				for(int xPos = 0; xPos < xMax; xPos++)
				{
					if (buttons [xPos, yPos].Id == "Letter")
					{
						if (characterSetIdx <= Characters.Symbols.Length - 1)
						{
							buttons [xPos, yPos].Text = characters.Symbols [characterSetIdx].ToString ();
						}
						characterSetIdx++;
					}
				}			
			}
			updateEntireKeyboard = true;
		}

	}

	internal class TextButton : Button
	{

		private string currentLine = "";
		private List<string> inputLines = new List<string>();
		private string resultString = "";
		private Font resultFont = Font.MediumFont;
		private bool useSmallFont = false;
		private bool showLine = false;
		private const int characterSize = 14;
		private Rectangle container;
		private const int characterEdge = 1;
		private Point start;

		public TextButton(string startText, Position position, Position size, Rectangle container): base (startText, position, size, container, "Text",true, ExitType.Center)
		{
			textCenterPoint = new Point (innerBox.P1.X + characterEdge *2, textCenterPoint.Y);


		}   

		public string SelectedCharacter{ get; set;}
		public string Text
		{
			get
			{
				return resultString;
			}
		}
		public override void Draw()
		{
			Lcd.DrawRectangle(outherBox, true, true);
			Lcd.DrawRectangle (innerBox, Selected, true);
			string character = SelectedCharacter;
			if (character == null)
			{
				character = " ";
			}
			if (!useSmallFont) 
			{
				Lcd.WriteText (resultFont, textCenterPoint, currentLine + SelectedCharacter, !Selected);
				int xUnderLine = innerBox.P1.X + characterEdge + resultFont.TextSize (currentLine).X;
				int yUnderLine = innerBox.P2.Y - 1;
				Lcd.DrawHLine (new Point (xUnderLine, yUnderLine), resultFont.TextSize(character).X, true);
			} 
			else 
			{
				if (showLine) 
				{
					if (inputLines.Count > 1) 
					{
						Point topPoint = new Point (innerBox.P1.X + characterEdge, innerBox.P1.Y);
						Lcd.WriteText (resultFont, topPoint, inputLines [inputLines.Count - 2], !Selected);
						Lcd.WriteText (resultFont, topPoint + new Point (0, ((int)resultFont.maxHeight-1)), inputLines [inputLines.Count - 1], !Selected);
						Lcd.WriteText (resultFont, topPoint + new Point (0, ((int)resultFont.maxHeight-1) * 2), currentLine + SelectedCharacter, !Selected);

					} 
					else 
					{
						Point topPoint = new Point (innerBox.P1.X + characterEdge, textCenterPoint.Y);
						Lcd.WriteText (resultFont, topPoint, inputLines [inputLines.Count - 1], !Selected);
						Lcd.WriteText (resultFont, topPoint + new Point (0, (int)resultFont.maxHeight-1), currentLine + SelectedCharacter, !Selected);
						int xUnderLine = innerBox.P1.X + characterEdge + resultFont.TextSize (currentLine).X;
						int yUnderLine = innerBox.P2.Y - 1;
						Lcd.DrawHLine (new Point (xUnderLine, yUnderLine), resultFont.TextSize(character).X, true);
					}
				} 
				else 
				{
					Lcd.WriteText (resultFont, textCenterPoint + new Point(0,5), currentLine + SelectedCharacter, !Selected);
					int xUnderLine = innerBox.P1.X + characterEdge + resultFont.TextSize (currentLine).X;
					int yUnderLine = innerBox.P2.Y - 1;
					Lcd.DrawHLine (new Point (xUnderLine, yUnderLine), resultFont.TextSize(character).X, true);
				}
			}
			 
		}

		public void AddLine()
		{
			resultString = resultString + Environment.NewLine;
			inputLines.Add (currentLine);
			currentLine = "";
			showLine = true;
			useSmallFont = true;
			resultFont = Font.SmallFont;
		}

		public void AddSpace()
		{
			AddToResult (" ");	
		}

		public void AddSelectedCharacter ()
		{
			if (SelectedCharacter == null || SelectedCharacter == String.Empty)
				return;
			AddToResult (SelectedCharacter);
		}

		public void DeleteCharacter ()
		{
			if (resultString.Length != 0)
			{
				resultString = resultString.Substring(0, resultString.Length - 1);
			}
			if (currentLine.Length != 0) 
			{
				currentLine = currentLine.Substring (0, currentLine.Length - 1);
				if (useSmallFont)
				{
					if (inputLines.Count == 0)
					{
						bool tooBig =  Font.MediumFont.TextSize(currentLine).X >= (innerBox.P2.X - innerBox.P1.X) - characterSize;
						if (!tooBig) 
						{
							useSmallFont = false;
							resultFont = Font.MediumFont;
						} 
					} 
				}
			} 
			else if (useSmallFont && inputLines.Count != 0) 
			{
				currentLine = inputLines[inputLines.Count-1];
				inputLines.RemoveAt(inputLines.Count-1);
				if (currentLine.Length != 0) {
					currentLine = currentLine.Substring (0, currentLine.Length - 1);
				} 
				else 
				{
					currentLine = "";
				}
				if(inputLines.Count == 0)
					showLine = false;	
			}
		}

		private void AddToResult(string newString)
		{
			resultString = resultString + newString;
			currentLine = currentLine + newString;
			int charSize;
			if (useSmallFont) {
				charSize = characterSize/2;
			} 
			else 
			{
				charSize = characterSize;
			}
			bool tooBig = resultFont.TextSize (currentLine).X >= (innerBox.P2.X - innerBox.P1.X) - charSize;
			if (tooBig) 
			{ 
				if (!useSmallFont) 
				{
					useSmallFont = true;
					resultFont = Font.SmallFont;
				} 
				else 
				{
					if (tooBig) {//add using small font 
						showLine = true;
						inputLines.Add (currentLine);
						currentLine = "";		
					}
				}
			}
		}


	}



}

