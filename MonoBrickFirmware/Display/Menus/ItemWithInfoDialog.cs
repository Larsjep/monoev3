using System;
using MonoBrickFirmware.Display.Dialogs;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithInfoDialog : ItemWithDialog<InfoDialog>
	{
		ItemWithInfoDialog(string message, bool showOk, string title): base(new InfoDialog(message, showOk), title)
		{

		}

		public override void OnExit (InfoDialog dialog)
		{

		}
	}
}

