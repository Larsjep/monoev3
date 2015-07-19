using System;
using MonoBrickFirmware.Display.Menus;

namespace StartupApp
{
	public class FirmwareMenuContainer : MenuContainer 
	{
		public FirmwareMenuContainer (Menu menu) : base(menu)
		{
		}

		public void Show(int startIndex)
		{
			topMenu.OnDrawContent ();
			for (int i = 0; i < startIndex; i++) 
			{
				topMenu.OnDownPressed ();		
			}
			topMenu.OnEnterPressed ();
			for (int i = 0; i < startIndex; i++) 
			{
				topMenu.OnUpPressed ();	
			}
			Show ();
		}
	}
}

