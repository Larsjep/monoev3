using System;
using MonoBrickFirmware.Display;

namespace MonoBrickFirmware.Display.Menus
{

	public class ItemWithCheckBox : IChildItem
	{
		private string text;
		private const int lineSize = 2;
		private const int edgeSize = 2;
		private bool isChecked;
		public Action<bool> OnCheckedChanged = delegate {};

		public ItemWithCheckBox (string text, bool checkedAtStart){
			this.text = text;
			this.Checked = checkedAtStart;
		}

		public IParentItem Parent { get; set;}
		
		public virtual void OnEnterPressed ()
		{
			Checked = !Checked;
		}
		
		public void OnDrawTitle (Font f, Rectangle r, bool color)
		{
			int xCheckBoxSize =(int) f.maxWidth;
			Rectangle outer = new Rectangle(new Point(Lcd.Width - xCheckBoxSize + edgeSize, r.P1.Y + edgeSize), new Point(r.P2.X - edgeSize,r.P2.Y - edgeSize));
			Rectangle innter = new Rectangle(new Point(Lcd.Width - xCheckBoxSize + lineSize+edgeSize, r.P1.Y+lineSize + edgeSize), new Point(r.P2.X - lineSize - edgeSize,r.P2.Y - lineSize - edgeSize));
			Point fontPoint = f.TextSize("v");
			Point checkPoint = new Point(Lcd.Width - xCheckBoxSize +(int) fontPoint.X-edgeSize, r.P1.Y);
			
			Lcd.WriteTextBox(f, r, text, color);
			Lcd.DrawRectangle(outer,color, true);
			Lcd.DrawRectangle(innter,!color, true);
			if(Checked)
				Lcd.WriteText(f,checkPoint,"v", color);
		}

		public void OnDrawContent ()
		{

		}

		public void OnLeftPressed ()
		{
			
		}

		public void OnRightPressed()
		{
			
		}

		public virtual void OnHideContent ()
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

		public bool Checked{get{return isChecked;}internal set{isChecked = value; OnCheckedChanged (isChecked);}}
	}

}

