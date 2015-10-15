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

		private bool useEscForTermination = true;
		private List<IChildItem> childItems = new List<IChildItem>();

		public Menu(string title) : base(title, Font.MediumFont, false)
		{
								
		}

		protected override List<IChildItem> OnCreateChildList ()
		{
			return childItems;
		}

		public void AddItem(IChildItem item)
		{
			childItems.Add (item);
		}

		public override void OnEscPressed ()
		{
			if (useEscForTermination && show) 
			{
				show = false;
				Parent.RemoveFocus (this);
			} 
		}

		internal void SetAsTopMenu(bool useEscForTermination)
		{
			this.useEscForTermination = useEscForTermination;
			show = true;
		}
	}
}

