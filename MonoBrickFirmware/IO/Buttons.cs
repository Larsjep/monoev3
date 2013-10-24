using System;
using MonoBrickFirmware.Native;
using System.Threading;

namespace MonoBrickFirmware.IO
{
	public class Buttons : IDisposable
	{
		[Flags]
		public enum ButtonStates
		{
			None		= 0x00,
			Up			= 0x01,
			Enter		= 0x02,
			Down		= 0x04,
			Right		= 0x08,
			Left		= 0x10,
			Escape		= 0x20,
			All			= 0xff,
		}		
		public const int ButtonCount = 6;
		
		
		UnixDevice dev;
		MemoryArea buttonMem;
		const int debounceTime = 10;
		const int pollTime = 50;
		public Buttons ()
		{
			dev = new UnixDevice("/dev/lms_ui");
			buttonMem = dev.MMap(ButtonCount, 0);			
		}
				
		ButtonStates ReadButtons()
		{
			ButtonStates bs = ButtonStates.None;
			byte[] buttonData = buttonMem.Read();
			int bitMask = 1;
			foreach (byte butState in buttonData)
			{
				if (butState != 0)
					bs |= (ButtonStates)bitMask;
				bitMask = bitMask << 1;
			}
			return bs;
		}
		
		internal ButtonStates GetDebounced()
		{
			
			ButtonStates s2 = ButtonStates.None;
			for (;;)
			{
				ButtonStates s1 = ReadButtons();
				if (s1 == s2)
					return s1;
				Thread.Sleep(debounceTime);
				s2 = ReadButtons();
			}
				
		}
		
		public ButtonStates GetKeypress()
		{
			ButtonStates bs = GetDebounced();
			while (bs != ButtonStates.None)
			{
				bs = GetDebounced();
			}
			do
			{				
				Thread.Sleep(pollTime);
				bs = GetDebounced();
			} while (bs == ButtonStates.None);
			return bs;
		}			
		
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			dev.Dispose();
		}
		#endregion
	}
	
	public class ButtonEvents : IDisposable
	{
		EventWaitHandle stopPolling = new ManualResetEvent(false);
		const int pollTime = 50;
		Buttons btns = new Buttons();
		
		public ButtonEvents()
		{
			new Thread(ButtonPollThread).Start();
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
		
		void ButtonPollThread()
		{	
			Thread.CurrentThread.IsBackground = true;
			Buttons.ButtonStates lastState = btns.GetDebounced();
			while (!stopPolling.WaitOne(pollTime))
			{
				Buttons.ButtonStates bs = btns.GetDebounced();
				if (bs != lastState)
				{
					Buttons.ButtonStates pressed = (bs ^ lastState) & (~lastState);
					switch (pressed)
					{
						case Buttons.ButtonStates.Down:		DownPressed(); break;
						case Buttons.ButtonStates.Enter:	EnterPressed(); break;
						case Buttons.ButtonStates.Escape:	EscapePressed(); break;
						case Buttons.ButtonStates.Left:		LeftPressed(); break;
						case Buttons.ButtonStates.Right:	RightPressed(); break;
						case Buttons.ButtonStates.Up:		UpPressed(); break;
					}
					
					Buttons.ButtonStates released = (bs ^ lastState) & lastState;
					switch (released)
					{
						case Buttons.ButtonStates.Down:		DownReleased(); break;
						case Buttons.ButtonStates.Enter:	EnterReleased(); break;
						case Buttons.ButtonStates.Escape:	EscapeReleased(); break;
						case Buttons.ButtonStates.Left:		LeftReleased(); break;
						case Buttons.ButtonStates.Right:	RightReleased(); break;
						case Buttons.ButtonStates.Up:		UpReleased(); break;
					}
					lastState = bs;
				}
				
			}
		}
		
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			((IDisposable)btns).Dispose();
		}
		#endregion
	}
}

