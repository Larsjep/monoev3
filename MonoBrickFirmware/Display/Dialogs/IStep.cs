using System;

namespace MonoBrickFirmware.Display.Dialogs
{
	public interface IStep
	{
		string Text{get;}
		string ErrorText{get;}
		bool Execute ();
	}
}

