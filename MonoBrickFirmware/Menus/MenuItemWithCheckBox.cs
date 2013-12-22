using System;
using MonoBrickFirmware.Display;

namespace MonoBrickFirmware.Menus
{
	public class MenuItemWithCheckBox : IMenuItem
	{
		private string text;
		private Lcd lcd;
		private const int lineSize = 2;
		private const int edgeSize = 2;
		private Func<bool,bool>func;
		public Action<bool> OnCheckedChanged = delegate {};
		public MenuItemWithCheckBox (Lcd lcd, string text, bool checkedAtStart, Func<bool,bool>enterFunc = null){
			this.text = text;
			this.lcd = lcd;
			this.Checked = checkedAtStart;
			this.func = enterFunc;
		}
		public bool EnterAction ()
		{
			if (func != null) {
				Checked = func(Checked);
				OnCheckedChanged(Checked);	
			} 
			else 
			{
				Checked = !Checked;
				OnCheckedChanged(Checked);
			}
			return false;
		}
		public bool LeftAction (){return false;}
		public bool RightAction(){return false;}
		public void Draw (Font f, Rectangle r, bool color)
		{
			int xCheckBoxSize =(int) f.maxWidth;
			Rectangle outer = new Rectangle(new Point(Lcd.Width - xCheckBoxSize + edgeSize, r.P1.Y + edgeSize), new Point(r.P2.X - edgeSize,r.P2.Y - edgeSize));
			Rectangle innter = new Rectangle(new Point(Lcd.Width - xCheckBoxSize + lineSize+edgeSize, r.P1.Y+lineSize + edgeSize), new Point(r.P2.X - lineSize - edgeSize,r.P2.Y - lineSize - edgeSize));
			Point fontPoint = f.TextSize("v");
			Point checkPoint = new Point(Lcd.Width - xCheckBoxSize +(int) fontPoint.X-edgeSize, r.P1.Y);
			
			lcd.WriteTextBox(f, r, text, color);
			lcd.DrawBox(outer,color);
			lcd.DrawBox(innter,!color);
			if(Checked)
				lcd.WriteText(f,checkPoint,"v", color);
		}
		public bool Checked{get;private set;}
	}
}

