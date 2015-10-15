using System;
using System.Threading;

namespace MonoBrickFirmware.UserInput
{
	public interface IButtons
	{
		Buttons.ButtonStates GetStates ();
		void WaitForKeyRelease (CancellationToken token);
		void WaitForKeyRelease ();
		Buttons.ButtonStates GetKeypress (CancellationToken token);
		Buttons.ButtonStates GetKeypress ();
		void LedPattern (int pattern);
	}

}

