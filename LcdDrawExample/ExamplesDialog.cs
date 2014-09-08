using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Display.Dialogs;

namespace LcdDraw
{
	public class ExamplesDialog: Dialog{

		private int _currentScreen;
		private Point _center;

		public ExamplesDialog(string title) : 
			base(Font.MediumFont, title, Lcd.Width, Lcd.Height-(int)Font.MediumFont.maxHeight)
		{
			_currentScreen = 1;
			_center = new Point (Lcd.Width / 2, Lcd.Height / 2);
		}

		protected override bool OnEnterAction ()
		{
			_currentScreen = (_currentScreen == 1 ? 2 : 1);
			Lcd.Instance.Clear ();

			return false; // no exit
		}

		protected override bool OnEscape()
		{
			return true; // exit
		}

		protected override void OnDrawContent ()
		{
			if (_currentScreen == 1)
				DrawFirstScreen ();
			else
				DrawSecondScreen ();
		}

		private void DrawFirstScreen()
		{
			Lcd.Instance.DrawLine( new Point (0, 0), new Point (Lcd.Width-1, Lcd.Height-1), true);
			Lcd.Instance.DrawLine( new Point (0, Lcd.Height-1), new Point (Lcd.Width-1, 0), true);
			Lcd.Instance.DrawLine( new Point (40, 40), new Point (Lcd.Width-40, Lcd.Height-40), true);
			Lcd.Instance.DrawLine( new Point (40, 100), new Point (Lcd.Width-20, Lcd.Height-60), true);

			Lcd.Instance.DrawCircle (_center, 60, true);
			Lcd.Instance.DrawCircle (_center, 50, true);
			Lcd.Instance.DrawCircle (_center, 40, true);
			Lcd.Instance.DrawCircleFilled (_center, 30, true);
			Lcd.Instance.DrawCircleFilled (_center, 20, false);
			Lcd.Instance.DrawCircleFilled (_center, 10, true);

			Lcd.Instance.DrawEllipse (_center, 60, 30, true);
		}

		private void DrawSecondScreen()
		{
			Lcd.Instance.DrawEllipseFilled (_center, 60, 30, true);
			Lcd.Instance.DrawEllipseFilled (new Point(0, Lcd.Height/2) , 60, 30, false);

			var r = new Rectangle (new Point (15, 28), new Point (Lcd.Width - 15, Lcd.Height - 25));
			Lcd.Instance.DrawRectangle (r, true);
		}

	}
}

