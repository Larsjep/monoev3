using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace MonoBrickFirmware.Dialogs
{
	
	public class CharacterDialog : Dialog
    {
		private const int maxNumberOfHorizontalCharacters = 10;
		private const int maxNumberOfLines = 4;
		private const int maxNumberOfButtomCharacters = maxNumberOfHorizontalCharacters -4; 
		private const int charactersInSet = maxNumberOfHorizontalCharacters*2 + maxNumberOfButtomCharacters;
		private const int characterSize = 14; 
		private const int characterEdge = 1;
		private const int characterOffset = 4;
		private const int lineThreeCharacterIndexStart = 2;
		private const int lineThreeCharacterIndexEnd = 8;
		private const int lineFourSpaceStart = 2;
		private const int lineFourSpaceEnd = 8;
		
		
		Point characterInnerBox; 
		Point characterOutherBox;
		private Point characterSpace = new Point(1,0);
		
		internal enum Selection {LeftArrow = 1, RightArrow = 2, Character = 3, Ok = 4, Delete = 5, Space = 6, Change = 7};
		private int selectedLine = 0;
		private int selectedIndex = 0;
		private char selectedCharacter = (char)0;
		private Selection selection = Selection.Character;
		private ICharacterSet[] alfabetSet;
		private ICharacterSet[] symboleSet;
		private bool showAlfabet = true;
		private int selectedSetIndex = 0;
		private ICharacterSet selectedSet;
		private const string alfabetSetString = "abc";
		private const string symboleSetString = "123";
		private string setTypeString;
		
		public CharacterDialog(Lcd lcd, Buttons btns, string title) : base(Font.MediumFont, lcd, btns, title, Lcd.Width, Lcd.Height-22) 
        {
			characterInnerBox = new Point(characterSize, characterSize); 
			characterOutherBox = new Point(characterInnerBox.X + 2* characterEdge, characterInnerBox.Y + 2* characterEdge);
			alfabetSet = new ICharacterSet[]{new SmallLetters(), new BigLetters()};
			symboleSet = new ICharacterSet[]{new NumbersAndSymbols(), new NumbersAndSymbols2()};
			selectedSet = alfabetSet[selectedSetIndex];
			setTypeString = symboleSetString;
        } 
        
		private bool ChangeSetType ()
		{
			selectedSetIndex = 0;
			if (showAlfabet) {
				selectedSet = symboleSet [selectedSetIndex];
				showAlfabet = false;
				setTypeString = alfabetSetString;
			} 
			else 
			{
				selectedSet = alfabetSet [selectedSetIndex];
				showAlfabet = true;
				setTypeString = symboleSetString;
			}
			return false;
		}
		
		private bool AddCharacter(char letter)
		{
			return false;
		}
    	
		private bool DeleteCharacter ()
		{
			return false;
		}
		
		private bool NextSet ()
		{
			if (showAlfabet) {
				selectedSetIndex++;
				if(selectedSetIndex >= alfabetSet.Length)
					selectedSetIndex = 0;
				selectedSet = alfabetSet[selectedSetIndex];	
			} 
			else 
			{
				selectedSetIndex++;
				if(selectedSetIndex >= symboleSet.Length)
					selectedSetIndex = 0;
				selectedSet = symboleSet[selectedSetIndex];
			}
			return false;
		}
		
		private bool PreviousSet ()
		{
			if (showAlfabet) {
				selectedSetIndex--;
				if(selectedSetIndex < 0)
					selectedSetIndex = alfabetSet.Length-1;
				selectedSet = alfabetSet[selectedSetIndex];	
			} 
			else 
			{
				selectedSetIndex--;
				if(selectedSetIndex < 0)
					selectedSetIndex = symboleSet.Length-1;
				selectedSet = symboleSet[selectedSetIndex];
			}
			return false;
		
		}
		
		private bool Done(){
			return true;
		
		}
        
		protected override bool OnEnterAction ()
		{
			bool end = false;
			switch (selection) 
			{
				case Selection.Change:
					end = ChangeSetType();
					break;
				case Selection.Character:
					end = AddCharacter(selectedCharacter);
					break;
				case Selection.Delete:
					end = DeleteCharacter();
					break;
				case Selection.LeftArrow:
					end = PreviousSet();
					break;
				case Selection.Ok:
					end = Done();
					break;
				case Selection.RightArrow:
					end = NextSet();
					break;
				case Selection.Space:
					end = AddCharacter(' ');
					break;
			}
			return end;
		}
		
		protected override bool OnLeftAction ()
		{
			switch (selection) 
			{
				case Selection.Delete:
					selectedIndex = lineThreeCharacterIndexEnd-1;
					break;
				case Selection.Ok:
					selectedIndex = lineFourSpaceEnd-1;
					break;
				case Selection.Space:
					selectedIndex = lineFourSpaceStart-1;
					break;
				default:
					selectedIndex--;
					if(selectedIndex < 0)
						selectedIndex = 0;
					break;
			}
			return false;
		}
		
		protected override bool OnRightAction ()
		{
			switch (selection) 
			{
				case Selection.Change:
					selectedIndex = lineFourSpaceStart;
					break;
				case Selection.Delete:
					selectedIndex = lineFourSpaceStart;
					break;
				case Selection.Space:
					selectedIndex = lineFourSpaceEnd+1;
					break;
				default :
					selectedIndex++;
					if(selectedIndex > maxNumberOfHorizontalCharacters-1)
						selectedIndex = maxNumberOfHorizontalCharacters-1;
				break;
			}
			return false;
		}
		
		protected override bool OnUpAction ()
		{
			switch (selection) 
			{
				case Selection.Change:
					selectedIndex = 0;
					break;
				case Selection.Delete:
					selectedIndex = maxNumberOfHorizontalCharacters-1;
					break;
				case Selection.Space:
					selectedIndex = (maxNumberOfHorizontalCharacters-1)/2;
					break;
			}
			selectedLine--;
			if(selectedLine < 0)
				selectedLine = 0;
			return false;
		}
		
		protected override bool OnDownAction ()
		{
			selectedLine++;
			if(selectedLine > maxNumberOfLines-1)
				selectedLine = maxNumberOfLines-1;
			return false;	
		}
		
		protected override void OnDrawContent ()
		{
			bool rightArrowSelected = false;
			bool leftArrowSelected = false;
			bool deleteSelected = false;
			bool okSelected = false;
			bool spaceSelected = false;
			bool changeSelected = false;
			bool charactersSelected = false;
			
			int characterIndex = 0;
			Point start;
			Rectangle outherRect;
			Rectangle innerRect;
			int line;
			for (line = 0; line < 2; line++) {
				start = new Point (dialogWindowInner.P1.X, dialogWindowInner.P1.Y + (int)font.maxHeight / 2 -2 + line * characterOutherBox.Y);  
				outherRect = new Rectangle (start, start + characterOutherBox);
				for (int character = 0; character < maxNumberOfHorizontalCharacters; character++) {
					bool selected = (selectedLine == line && selectedIndex == character);
					if (selected) 
					{
						selection = Selection.Character;
						charactersSelected = true;
					}
					lcd.DrawBox (outherRect, true);
					innerRect = new Rectangle (new Point (outherRect.P1.X + characterEdge, outherRect.P1.Y + characterEdge), new Point (outherRect.P2.X - characterEdge, outherRect.P2.Y - characterEdge));
					lcd.DrawBox (innerRect, selected);
					lcd.WriteText (Font.MediumFont, new Point (innerRect.P1.X + characterOffset, innerRect.P1.Y - characterOffset), selectedSet.Characters[characterIndex].ToString(), !selected);
					outherRect = outherRect + new Point (characterOutherBox.X, 0) + characterSpace;
					characterIndex++;	
				}
			}
			if (selectedLine == line && selectedIndex < lineThreeCharacterIndexStart) {
				if (selectedIndex == 0) {
					selection = Selection.LeftArrow;
					leftArrowSelected = true;
				} 
				else 
				{
					selection = Selection.RightArrow;
					rightArrowSelected = true;
				}
			}
			start = new Point (dialogWindowInner.P1.X, dialogWindowInner.P1.Y + (int)font.maxHeight / 2 + line * characterOutherBox.Y);  
			
			outherRect = new Rectangle (start, start + characterOutherBox);
			lcd.DrawBox (outherRect, true);
			innerRect = new Rectangle (new Point (outherRect.P1.X + characterEdge, outherRect.P1.Y + characterEdge), new Point (outherRect.P2.X - characterEdge, outherRect.P2.Y - characterEdge));
			lcd.DrawBox (innerRect, leftArrowSelected);
			innerRect = new Rectangle (new Point (outherRect.P1.X + 3 * characterEdge, outherRect.P1.Y + 3 * characterEdge), new Point (outherRect.P2.X - 3 * characterEdge, outherRect.P2.Y - 3 * characterEdge));
			lcd.DrawArrow (innerRect, Lcd.ArrowOrientation.Left, !leftArrowSelected);
			
			outherRect = outherRect + new Point (characterOutherBox.X, 0) + characterSpace;
			lcd.DrawBox (outherRect, true);
			innerRect = new Rectangle (new Point (outherRect.P1.X + characterEdge, outherRect.P1.Y + characterEdge), new Point (outherRect.P2.X - characterEdge, outherRect.P2.Y - characterEdge));
			lcd.DrawBox (innerRect, rightArrowSelected);
			innerRect = new Rectangle (new Point (outherRect.P1.X + 3 * characterEdge, outherRect.P1.Y + 3 * characterEdge), new Point (outherRect.P2.X - 3 * characterEdge, outherRect.P2.Y - 3 * characterEdge));
			lcd.DrawArrow (innerRect, Lcd.ArrowOrientation.Right, !rightArrowSelected);
			
			for (int character = 0; character < maxNumberOfButtomCharacters; character++) {
				bool selected = false;
				if (selectedLine == line) {
					if (selectedIndex >= lineThreeCharacterIndexStart && selectedIndex < lineThreeCharacterIndexEnd && (selectedIndex - lineThreeCharacterIndexStart) == character) {
						selected = true;
						selection = Selection.Character;
						charactersSelected = true;
					}
				}
				outherRect = outherRect + new Point (characterOutherBox.X, 0) + characterSpace;
				lcd.DrawBox (outherRect, true);
				innerRect = new Rectangle (new Point (outherRect.P1.X + characterEdge, outherRect.P1.Y + characterEdge), new Point (outherRect.P2.X - characterEdge, outherRect.P2.Y - characterEdge));
				lcd.DrawBox (innerRect, selected);
				lcd.WriteText (Font.MediumFont, new Point (innerRect.P1.X + characterOffset, innerRect.P1.Y - characterOffset), selectedSet.Characters[characterIndex].ToString(), !selected);
				characterIndex++;	
			}
			
			if (selectedLine == line && selectedIndex >= lineThreeCharacterIndexEnd) {
				selection = Selection.Delete;
				deleteSelected = true;
			}
			
			outherRect =  new Rectangle(new Point(outherRect.P1.X, outherRect.P1.Y),new Point(outherRect.P2.X + characterOutherBox.X + characterSpace.X, outherRect.P2.Y)) +  new Point (characterOutherBox.X, 0) + new Point(characterSpace.X, characterSpace.Y);
			lcd.DrawBox (outherRect, true);
			innerRect = new Rectangle (new Point (outherRect.P1.X + characterEdge, outherRect.P1.Y + characterEdge), new Point (outherRect.P2.X - characterEdge, outherRect.P2.Y - characterEdge));
			lcd.DrawBox (innerRect, deleteSelected);
			lcd.WriteText (Font.MediumFont, new Point (innerRect.P1.X + characterOffset+1, innerRect.P1.Y - characterOffset+1), "Del", !deleteSelected);
			
			line++;
			
			if (selectedLine == line && selectedIndex >= lineFourSpaceEnd) {
				selection = Selection.Ok;
				okSelected = true;
			}
			if (selectedLine == line && selectedIndex < lineFourSpaceStart) {
				selection = Selection.Change;
				changeSelected = true;
			}
			
			if (selectedLine == line && selectedIndex >= lineFourSpaceStart && selectedIndex < lineFourSpaceEnd) {
				selection = Selection.Space;
				spaceSelected = true;
			}
			
			start = new Point (dialogWindowInner.P1.X, dialogWindowInner.P1.Y + (int)font.maxHeight / 2 + line*characterOutherBox.Y);  
			
			outherRect = new Rectangle (new Point(start.X, start.Y), new Point(start.X + 2*characterOutherBox.X + characterSpace.X, start.Y + characterOutherBox.Y));
			lcd.DrawBox (outherRect, true);
			innerRect = new Rectangle (new Point (outherRect.P1.X + characterEdge, outherRect.P1.Y + characterEdge), new Point (outherRect.P2.X - characterEdge, outherRect.P2.Y - characterEdge));
			lcd.DrawBox (innerRect, changeSelected);
			lcd.WriteText (Font.MediumFont, new Point (innerRect.P1.X + characterOffset-3, innerRect.P1.Y - characterOffset), setTypeString, !changeSelected);
			
			outherRect =  new Rectangle(new Point(start.X + 2*characterSpace.X + 2*characterOutherBox.X, start.Y),new Point(start.X + 2*characterSpace.X + 2*characterOutherBox.X + 6*characterOutherBox.X + 5* characterSpace.X, start.Y + characterOutherBox.Y));
			lcd.DrawBox (outherRect, true);
			innerRect = new Rectangle (new Point (outherRect.P1.X + characterEdge, outherRect.P1.Y + characterEdge), new Point (outherRect.P2.X - characterEdge, outherRect.P2.Y - characterEdge));
			lcd.DrawBox (innerRect, spaceSelected);
			lcd.WriteText (Font.MediumFont, new Point (innerRect.P1.X + characterOffset+3, innerRect.P1.Y - characterOffset+1), "  SPACE", !spaceSelected);
			
			outherRect =  new Rectangle(new Point(outherRect.P2.X + characterSpace.X, outherRect.P1.Y),new Point(outherRect.P2.X + 2*characterSpace.X + 2*characterOutherBox.X, outherRect.P2.Y));
			lcd.DrawBox (outherRect, true);
			innerRect = new Rectangle (new Point (outherRect.P1.X + characterEdge, outherRect.P1.Y + characterEdge), new Point (outherRect.P2.X - characterEdge, outherRect.P2.Y - characterEdge));
			lcd.DrawBox (innerRect, okSelected);
			lcd.WriteText (Font.MediumFont, new Point (innerRect.P1.X + characterOffset+2, innerRect.P1.Y - characterOffset+1), "OK", !okSelected);
			
        }
    }
    
    internal interface ICharacterSet
	{
		char[] Characters{ get;}
	}
	
	internal class BigLetters : ICharacterSet
	{
		public BigLetters ()
		{
			Characters = new char[26];
			char start = 'A';
			for (int i = 0; i < 26; i++) 
			{
				Characters[i] = start;
				start = (char)((int)start +1);
			}
		
		}
		public char[] Characters{ get; private set;}	
	}
	
	
	internal class SmallLetters : ICharacterSet
	{
		public SmallLetters ()
		{
			Characters = new char[26];
			char start = 'a';
			for (int i = 0; i < 26; i++) 
			{
				Characters[i] = start;
				start = (char)((int)start +1);
			}
		
		}
		public char[] Characters{ get; private set;}	
	}
	
	internal class NumbersAndSymbols: ICharacterSet
	{
		public NumbersAndSymbols()
		{
			Characters = new char[26];
			Characters[0] = '1';
			Characters[1] = '2';
			Characters[2] = '3';
			Characters[3] = '4';
			Characters[4] = '5';
			Characters[5] = '6';
			Characters[6] = '7';
			Characters[7] = '8';
			Characters[8] = '9';
			Characters[9] = '0';
			Characters[10] = '-';
			Characters[11] = '/';
			Characters[12] = '.';
			Characters[13] = ':';
			Characters[14] = ';';
			Characters[15] = '(';
			Characters[16] = ')';
			Characters[17] = '&';
			Characters[18] = '@';
			Characters[19] = '"';
			Characters[20] = '!';
			Characters[21] = '+';
			Characters[22] = '*';
			Characters[23] = ',';
			Characters[24] = '#';
			Characters[25] = '%';
			
		}
		public char[] Characters{ get; private set;}	
	}
	
	internal class NumbersAndSymbols2: ICharacterSet
	{
		public NumbersAndSymbols2()
		{
			Characters = new char[26];
			Characters[0] = '1';
			Characters[1] = '2';
			Characters[2] = '3';
			Characters[3] = '4';
			Characters[4] = '5';
			Characters[5] = '6';
			Characters[6] = '7';
			Characters[7] = '8';
			Characters[8] = '9';
			Characters[9] = '0';
			Characters[10] = '$';
			Characters[11] = (char) 39; //Single quote '
			Characters[12] = '<';
			Characters[13] = '=';
			Characters[14] = '>';
			Characters[15] = '?';
			Characters[16] = (char)92; //Backslash \
			Characters[17] = ']';
			Characters[18] = '^';
			Characters[19] = '_';
			Characters[20] = '`';
			Characters[21] = '{';
			Characters[22] = '|';
			Characters[23] = '}';
			Characters[24] = '~';
			Characters[25] = (char)0;
		}
		public char[] Characters{ get; private set;}	
	}
}

