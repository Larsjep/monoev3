using System;
using System.Threading;

namespace MonoBrickFirmware.Display.Dialogs
{
	public class StepContainer: IStep
	{
		private Func<bool> func;
		
		public StepContainer (Func<bool> stepToExecute, string text, string errorText): this(stepToExecute,text, errorText, "")
		{
		
		}
		
		public StepContainer (Func<bool> stepToExecute, string text, string errorText, string okText)
		{
			this.OkText = okText;
			StepText = text;
			ErrorText = errorText;
			func = stepToExecute;
			ShowOkText = (okText != "");
		}
		
		public string StepText{get; private set;}
		public string ErrorText{get; private set;}
		public string OkText{get; private set;}
		public bool ShowOkText{get; private set;}
		public bool Execute ()
		{
			return func();	
		}
		
	}
}

