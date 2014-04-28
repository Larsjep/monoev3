using System;
using MonoBrickFirmware.Display;

namespace MonoBrickFirmware.Display.Menus
{
	public class MenuItemWithCheckBox : IMenuItem
	{
		private string text;
		private const int lineSize = 2;
		private const int edgeSize = 2;
		private Func<bool,bool>func;
		public Action<bool> OnCheckedChanged = delegate {};
		
		public MenuItemWithCheckBox (string text, bool checkedAtStart, Func<bool,bool>enterFunc = null){
			this.text = text;
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
			
			Lcd.Instance.WriteTextBox(f, r, text, color);
			Lcd.Instance.DrawBox(outer,color);
			Lcd.Instance.DrawBox(innter,!color);
			if(Checked)
				Lcd.Instance.WriteText(f,checkPoint,"v", color);
		}
		public bool Checked{get;private set;}
	}
}

