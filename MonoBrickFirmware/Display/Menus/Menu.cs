using System;
using System.Collections.Generic;
using System.Linq;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using System.Threading;

namespace MonoBrickFirmware.Display.Menus
{
	
	public class Menu : ItemList
	{

		private bool topMenu = false;
		private List<IChildItem> childItems;

		public Menu(string title, List<IChildItem> childItems) : base(title, Font.MediumFont, false)
		{
			this.childItems = childItems;	
		}

		protected override List<IChildItem> OnCreateChildList ()
		{
			return childItems;
		}

		public override void OnEscPressed ()
		{
			if (!topMenu && show)
			{
				show = false;
				Parent.RemoveFocus(this);
			}	
		}

		public void SetAsTopMenu()
		{
			topMenu = true;	
		}
	}
}

