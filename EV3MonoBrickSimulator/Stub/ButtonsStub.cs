using System;
using MonoBrickFirmware.UserInput;
using System.Threading;

namespace  EV3MonoBrickSimulator.Stub
{
	public class ButtonsStub : IButtons
	{
		private Buttons.ButtonStates state;		
		private const int pollTime = 20;
		public Buttons.ButtonStates GetStates ()
		{
			return 	state;	
		}

		public void WaitForKeyRelease (System.Threading.CancellationToken token)
		{
			var bs = GetStates ();
			do {				
				Thread.Sleep (pollTime);
				bs = GetStates();
			} while (bs != Buttons.ButtonStates.None && !token.IsCancellationRequested);

		}

		public void WaitForKeyRelease ()
		{
			WaitForKeyRelease(new CancellationToken(false));
		}

		public Buttons.ButtonStates GetKeypress (System.Threading.CancellationToken token)
		{
			Buttons.ButtonStates bs = GetStates ();
			while (bs != Buttons.ButtonStates.None && !token.IsCancellationRequested)
			{
				bs = GetStates ();
			}
			do {				
				Thread.Sleep (pollTime);
				bs = GetStates();
			} while (bs == Buttons.ButtonStates.None && !token.IsCancellationRequested);
			return bs;
		}

		public Buttons.ButtonStates GetKeypress ()
		{
			return GetKeypress(new CancellationToken(false));	
		}

		public void LedPattern (int pattern)
		{

		}

		public void EnterPressed()
		{
			state = state | Buttons.ButtonStates.Enter;
		}
		public void EnterReleased()
		{
			state &= ~Buttons.ButtonStates.Enter;
		}
		public void DownPressed()
		{
			state = state | Buttons.ButtonStates.Down;
		}
		public void DownReleased()
		{
			state &= ~Buttons.ButtonStates.Down;
		}
		public void UpPressed()
		{
			state = state | Buttons.ButtonStates.Up;
		}
		public void UpReleased()
		{
			state &= ~Buttons.ButtonStates.Up;	
		}
		public void LeftPressed()
		{
			state = state | Buttons.ButtonStates.Left;
		}
		public void LeftReleased()
		{
			state &= ~Buttons.ButtonStates.Left;	
		}
		public void RightPressed()
		{
			state = state | Buttons.ButtonStates.Right;
		}
		public void RightReleased()
		{
			state &= ~Buttons.ButtonStates.Right;	
		}

		public void EscPressed()
		{
			state = state | Buttons.ButtonStates.Escape;
		}

		public void EscReleased()
		{
			state &= ~Buttons.ButtonStates.Escape;	
		}

	}
}

