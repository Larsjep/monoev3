using System;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Tools;
using System.Threading;

namespace MonoBrickFirmware.UserInput
{
	public class ButtonEvents : IDisposable
	{
		EventWaitHandle stopPolling = new ManualResetEvent (false);
		const int pollTime = 50;
		Buttons btns = new Buttons ();
		QueueThread queue = new QueueThread ();

		public ButtonEvents ()
		{
			new Thread (ButtonPollThread).Start ();
		}

		public event Action UpPressed = delegate {};
		public event Action EnterPressed = delegate {};
		public event Action DownPressed = delegate {};
		public event Action RightPressed = delegate {};
		public event Action LeftPressed = delegate {};
		public event Action EscapePressed = delegate {};
		public event Action UpReleased = delegate {};
		public event Action EnterReleased = delegate {};
		public event Action DownReleased = delegate {};
		public event Action RightReleased = delegate {};
		public event Action LeftReleased = delegate {};
		public event Action EscapeReleased = delegate {};

		void ButtonPollThread ()
		{	
			Thread.CurrentThread.IsBackground = true;
			Buttons.ButtonStates lastState = btns.GetDebounced ();
			while (!stopPolling.WaitOne (pollTime)) {
				Buttons.ButtonStates bs = btns.GetDebounced ();
				if (bs != lastState) {
					Buttons.ButtonStates pressed = (bs ^ lastState) & (~lastState);
					switch (pressed) {
					case Buttons.ButtonStates.Down:
						queue.Enqueue (DownPressed);
						break;
					case Buttons.ButtonStates.Enter:
						queue.Enqueue (EnterPressed);
						break;
					case Buttons.ButtonStates.Escape:
						queue.Enqueue (EscapePressed);
						break;
					case Buttons.ButtonStates.Left:
						queue.Enqueue (LeftPressed);
						break;
					case Buttons.ButtonStates.Right:
						queue.Enqueue (RightPressed);
						break;
					case Buttons.ButtonStates.Up:
						queue.Enqueue (UpPressed);
						break;
					}
					
					Buttons.ButtonStates released = (bs ^ lastState) & lastState;
					switch (released) {
					case Buttons.ButtonStates.Down:
						queue.Enqueue (DownReleased);
						break;
					case Buttons.ButtonStates.Enter:
						queue.Enqueue (EnterReleased);
						break;
					case Buttons.ButtonStates.Escape:
						queue.Enqueue (EscapeReleased);
						break;
					case Buttons.ButtonStates.Left:
						queue.Enqueue (LeftReleased);
						break;
					case Buttons.ButtonStates.Right:
						queue.Enqueue (RightReleased);
						break;
					case Buttons.ButtonStates.Up:
						queue.Enqueue (UpReleased);
						break;
					}
					lastState = bs;
				}
				
			}
		}

		#region IDisposable implementation

		void IDisposable.Dispose ()
		{
			((IDisposable)btns).Dispose ();
		}

		#endregion

	}

}

