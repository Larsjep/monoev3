using System;
using MonoBrickFirmware.Display;

namespace MonoBrickFirmware.Menus
{
	
	public interface IMenuItem{
		bool EnterAction();
		void Draw(Font f, Rectangle r, bool color);
		bool LeftAction();
		bool RightAction();
	}
}

