using System;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class StepContainer: IStep
	{
		private Func<bool> func;
		public StepContainer (Func<bool> stepToExecute, string text, string errorText)
		{
			Text = text;
			ErrorText = errorText;
			func = stepToExecute;
		}
		
		public string Text{get; private set;}
		public string ErrorText{get; private set;}
		public bool Execute ()
		{
			return func();	
		}
		
	}
}

