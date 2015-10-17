using System;

namespace MonoBrickFirmware.Display.Dialogs.UserInput
{
	public interface ICharacters
	{
		char[] Symbols{ get;}
	}

	public class Characters : ICharacters
	{
		public Characters(char[] chars)
		{
			this.Symbols = chars;	
		}
		public char[] Symbols{ get; private set;}
	}


	public interface ICharacterSet
	{
		ICharacters Characters{ get;}
		ICharacters ShiftCharacters{get;}
	}

	public class Letters : ICharacterSet
	{
		public Letters ()
		{
			const int size = 26;
			var smallChars = new char[size];
			char start = 'a';
			for (int i = 0; i < size; i++) 
			{
				smallChars[i] = start;
				start = (char)((int)start +1);
			}
			this.Characters = new Characters (smallChars);


			var bigLetters = new char[size];
			start = 'A';
			for (int i = 0; i < size; i++) 
			{
				bigLetters[i] = start;
				start = (char)((int)start +1);
			}
			this.ShiftCharacters = new Characters (bigLetters); 
		}
		public ICharacters Characters{ get; private set;}
		public ICharacters ShiftCharacters{ get; private set;}
	}

	public class NumbersAndSymbols: ICharacterSet
	{
		public NumbersAndSymbols()
		{
			var symbols = new char[26];
			symbols[0] = '1';
			symbols[1] = '2';
			symbols[2] = '3';
			symbols[3] = '4';
			symbols[4] = '5';
			symbols[5] = '6';
			symbols[6] = '7';
			symbols[7] = '8';
			symbols[8] = '9';
			symbols[9] = '0';
			symbols[10] = '-';
			symbols[11] = '/';
			symbols[12] = '.';
			symbols[13] = ':';
			symbols[14] = ';';
			symbols[15] = '(';
			symbols[16] = ')';
			symbols[17] = '&';
			symbols[18] = '@';
			symbols[19] = '"';
			symbols[20] = '!';
			symbols[21] = '+';
			symbols[22] = '*';
			symbols[23] = ',';
			symbols[24] = '#';
			symbols[25] = '%';
			Characters = new Characters (symbols);
		}
		public ICharacters Characters{ get; private set;}
		public ICharacters ShiftCharacters{get{return null;}}
	}

	public class NumbersAndSymbols2: ICharacterSet
	{
		public NumbersAndSymbols2()
		{
			var symbols = new char[26];
			symbols[0] = '1';
			symbols[1] = '2';
			symbols[2] = '3';
			symbols[3] = '4';
			symbols[4] = '5';
			symbols[5] = '6';
			symbols[6] = '7';
			symbols[7] = '8';
			symbols[8] = '9';
			symbols[9] = '0';
			symbols[10] = '$';
			symbols[11] = (char) 39; //Single quote '
			symbols[12] = '<';
			symbols[13] = '=';
			symbols[14] = '>';
			symbols[15] = '?';
			symbols[16] = (char)92; //Backslash \
			symbols[17] = ']';
			symbols[18] = '^';
			symbols[19] = '_';
			symbols[20] = '`';
			symbols[21] = '{';
			symbols[22] = '|';
			symbols[23] = '}';
			symbols[24] = '~';
			symbols[25] = (char)0;
			Characters = new Characters (symbols);
		}
		public ICharacters Characters{ get; private set;}
		public ICharacters ShiftCharacters{get{return null;}}
	}
}

