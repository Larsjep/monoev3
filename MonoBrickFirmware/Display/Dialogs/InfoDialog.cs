using System;
using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using System.Collections.Generic;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class InfoDialog: Dialog{
		private List<string> stringList;
		private int pagePos;
		private const int scrollBarWidth = 5;
		private const int scrollBarOffSet = 2;
		private const int scrollIndexWith = 4;
		private int numberOfPages;
		private Rectangle scrollBar;
		private int indexHeight;
		public InfoDialog(string message, string title = "Information"):base(Font.MediumFont,title)
		{
			stringList = Text2List (message, 0);
			UpdateNumberOfPages ();
			if (numberOfPages > 1) {
				stringList = Text2List (message, scrollBarWidth + scrollBarOffSet);
				UpdateNumberOfPages ();
				scrollBar = new Rectangle (new Point (this.innerWindow.P2.X - scrollBarWidth - scrollBarOffSet, this.innerWindow.P1.Y + scrollBarOffSet), new Point (this.innerWindow.P2.X - scrollBarOffSet, this.innerWindow.P2.Y - scrollBarOffSet));
				for (int i = 0; i < lines.Count; i++)
				{
					Point p1 = lines [i].P1;
					Point p2 = lines [i].P2;
					lines.RemoveAt (i);
					lines.Insert(i, new Rectangle( p1, new Point(p2.X - (scrollBarWidth + scrollBarOffSet),p2.Y ))); 
				}
				indexHeight = (scrollBar.P2.Y - scrollBar.P1.Y) / numberOfPages;
			} 
		}

		internal override void OnEnterPressed ()
		{
			OnExit();
		}

		protected override void OnDrawContent ()
		{
			DrawText ();
			DrawCenterButton("OK", false);
		}

		internal override void OnUpPressed ()
		{
			if (pagePos > 0) 
			{
				pagePos--;
			}
		}

		internal override void OnDownPressed ()
		{
			if (pagePos < numberOfPages-1)
			{
				pagePos++;
			}
		}

		protected void DrawText()
		{
			if (numberOfPages == 1 && stringList.Count == 1) 
			{
				int middle = (lines.Count / 2);
				Lcd.WriteTextBox (font, lines[middle], stringList[0] , true, Lcd.Alignment.Center);
			} 
			else 
			{
				for (int i = 0; i != lines.Count; ++i) 
				{
					if (i + pagePos * lines.Count >= stringList.Count)
						break;
					WriteTextOnLine (stringList [i + pagePos * lines.Count], i, true, Lcd.Alignment.Center);
				}
				if (numberOfPages > 1)
				{
					DrawScrollBar ();
				}
			}
		}

		private void UpdateNumberOfPages()
		{
			numberOfPages = (stringList.Count / lines.Count);
			if (stringList.Count % lines.Count != 0) 
			{
				numberOfPages++;
			}	
		}

		private void DrawScrollBar()
		{
			Lcd.DrawRectangle (scrollBar, false, true);
			Lcd.DrawRectangle (scrollBar, true, false);
			Rectangle indexRec = new Rectangle (new Point(scrollBar.P1.X + (scrollBarWidth - scrollIndexWith)/2, scrollBar.P1.Y + pagePos * indexHeight), new Point(scrollBar.P2.X - (scrollBarWidth - scrollIndexWith)/2, scrollBar.P1.Y + pagePos * indexHeight + indexHeight ) );
			Lcd.DrawRectangle (indexRec, true, true);
		}

		private List<string> Text2List (string text, int offset)
		{
			List<string> stringList = new List<string> ();
			if(!String.IsNullOrEmpty(text))
			{

				int width = lines [0].P2.X - lines [0].P1.X - offset;
				int textRectRatio = font.TextSize (text).X / (width);
				if (textRectRatio == 0) 
				{
					stringList.Add (text);
				} 
				else 
				{
					string[] words = text.Split (' ');
					string s = "";
					for (int i = 0; i < words.Length; i++) {
						if (font.TextSize (s + " " + words [i]).X < width) {
							if (s == "") {
								s = words [i]; 
							} else {
								s = s + " " + words [i];
							}
						} else {
							stringList.Add (s);
							s = words [i];
						}  			

					}
					if (s != "") 
					{
						stringList.Add (s);
					}
				}
			}
			return stringList;
		}
	}
}

