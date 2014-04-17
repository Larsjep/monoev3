using System;

namespace MonoBrickFirmware.Display.Dialogs
{
	public interface IDialogStep
	{
		string Text{get;}
		string ErrorText{get;}
		bool Execute ();
	}
}

