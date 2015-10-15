using System;
using MonoBrickFirmware.Display;

namespace MonoBrickFirmware.Display.Menus
{

	public class ItemWithCheckBox : ChildItem
	{
		private string text;
		private const int lineSize = 2;
		private const int edgeSize = 2;
		private bool isChecked;
		protected Action<bool> OnCheckedChanged = delegate {};
		public ItemWithCheckBox (string text, bool checkedAtStart, Action<bool> OnCheckedChanged = null){
			this.text = text;
			this.Checked = checkedAtStart;
			this.OnCheckedChanged = OnCheckedChanged;
		}

		public override void OnEnterPressed ()
		{
			Checked = !Checked;
		}
		
		public override void OnDrawTitle (Font f, Rectangle r, bool color)
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

		public bool Checked
		{
			get{return isChecked;}
			internal 
			set{
				isChecked = value; 
				if (OnCheckedChanged != null) 
				{
					OnCheckedChanged(isChecked);
				} 
			}
		}
	}

}

