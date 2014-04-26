using System;

namespace MonoBrickFirmware.Display.Dialogs
{
	public interface IStep
	{
		string StepText{get;}
		string OkText{get;}
		string ErrorText{get;}
		bool Execute ();
		bool ShowOkText{get;}
	}
}

