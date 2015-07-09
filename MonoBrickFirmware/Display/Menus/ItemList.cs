using System;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using System.Threading;

namespace MonoBrickFirmware.Display.Menus
{

	public abstract class ItemList : IChildItem, IParentItem
	{
		private List<IChildItem> items = new List<IChildItem> ();
		private Point itemSize;
		private Point itemHeight;
		private int itemsOnScreen;
		private int cursorPos;
		private int scrollPos;
		private int arrowHeight = 5;
		private int arrowWidth = 10;
		private bool isItemSizeCalculated = false;
		private Font font;
		private string title;
		private const int arrowEdge = 4;
		private const int arrowOffset = 4;
		private bool hasLoadedList = false;
		private bool reloadOnFocus;
		protected bool show;

		public ItemList (string title, Font font, bool reloadListOnFocus = true)
		{
			this.title = title;
			this.font = font;
			this.reloadOnFocus = reloadListOnFocus;
		}

		public IParentItem Parent { get; set; }

		protected abstract List<IChildItem> OnCreateChildList ();

		public void OnUpPressed ()
		{
			if (cursorPos + scrollPos > 0) {
				if (cursorPos > 0)
					cursorPos--;
				else
					scrollPos--;
			}
		}

		public void OnDownPressed ()
		{
			if (scrollPos + cursorPos < items.Count - 1) {
				if (cursorPos < itemsOnScreen - 1)
					cursorPos++;
				else
					scrollPos++;
			}
		}

		public void OnDrawTitle (Font font, Rectangle rectangle, bool selected)
		{
			Lcd.WriteTextBox (font, rectangle, title, selected);
			int arrowWidth = (int)font.maxWidth / 3;
			Rectangle arrowRect = new Rectangle (new Point (rectangle.P2.X - (arrowWidth + arrowOffset), rectangle.P1.Y + arrowEdge), new Point (rectangle.P2.X - arrowOffset, rectangle.P2.Y - arrowEdge));
			Lcd.DrawArrow (arrowRect, Lcd.ArrowOrientation.Right, selected);
		}


		public void OnDrawContent ()
		{
			if (!hasLoadedList)
			{
				CreateNewList (OnCreateChildList());
			}
			DrawItemList ();
		}

		public virtual void OnEnterPressed ()
		{
			if (!show) {
				show = true;
				if (!hasLoadedList || reloadOnFocus) 
				{
					CreateNewList (OnCreateChildList());		
				}
				Parent.SetFocus (this);
			} 
			else 
			{
				items [scrollPos + cursorPos].OnEnterPressed ();
			}
		}

		public virtual void OnLeftPressed ()
		{
			if (show) 
			{
				items [scrollPos + cursorPos].OnLeftPressed ();
			}
		}

		public virtual void OnRightPressed ()
		{
			
			if (!show) 
			{
				show = true;
				if (!hasLoadedList || reloadOnFocus) 
				{
					CreateNewList (OnCreateChildList());		
				}
				Parent.SetFocus (this);
			} 
			else 
			{
				items [scrollPos + cursorPos].OnRightPressed ();
			}

		}

		public virtual void OnEscPressed ()
		{
			if (show) 
			{
				show = false;
				Parent.RemoveFocus (this);
			} 	
		}


		public virtual void OnHideContent ()
		{

		}

		#region IParentMenuItem implementation

		public void SetFocus (IChildItem item)
		{
			show = false;
			Parent.SetFocus (item);
		}

		public void RemoveFocus (IChildItem item)
		{
			show = true;
			if (!hasLoadedList || reloadOnFocus) 
			{
				CreateNewList (OnCreateChildList());		
			}
			Parent.SetFocus (this);
		}


		public void SuspendEvents (IChildItem item)
		{
			Parent.SuspendEvents (item);
		}

		public void ResumeEvents (IChildItem item)
		{
			Parent.ResumeEvents (item);
		}
		#endregion

		private void CalculateItemSize ()
		{
			this.itemSize = new Point (Lcd.Width, (int)font.maxHeight);
			this.itemHeight = new Point (0, (int)font.maxHeight);
			this.itemsOnScreen = (int)((Lcd.Height - arrowHeight) / font.maxHeight - 1); // -1 Because of the title
			cursorPos = 0;
			scrollPos = 0;
		}

		private void DrawItemList ()
		{
			if (!isItemSizeCalculated) {
				CalculateItemSize ();
				isItemSizeCalculated = true;
			}
			Lcd.Clear ();
			Rectangle currentPos = new Rectangle (new Point (0, 0), itemSize);
			Rectangle arrowRect = new Rectangle (new Point (Lcd.Width / 2 - arrowWidth / 2, Lcd.Height - arrowHeight), new Point (Lcd.Width / 2 + arrowWidth / 2, Lcd.Height - 1));

			Lcd.WriteTextBox (font, currentPos, title, true, Lcd.Alignment.Center);
			int i = 0;
			while (i != itemsOnScreen) {
				if (i + scrollPos >= items.Count)
					break;
				items [i + scrollPos].OnDrawTitle (font, currentPos + itemHeight * (i + 1), i != cursorPos);
				i++;
			}
			Lcd.DrawArrow (arrowRect, Lcd.ArrowOrientation.Down, scrollPos + itemsOnScreen < items.Count);
			Lcd.Update ();
		}

		private void CreateNewList(List<IChildItem> newList)
		{
			items.Clear();
			foreach (var i in newList) 
			{
				items.Add (i);
				i.Parent = this;
			}
			hasLoadedList = true;
		}
	}
}

