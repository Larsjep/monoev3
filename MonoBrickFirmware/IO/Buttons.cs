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
		public event Action UpPressed = delegate {};
		public event Action EnterPressed = delegate {};
		public event Action DownPressed = delegate {};
		public event Action RightPressed = delegate {};
		public event Action LeftPressed = delegate {};
		public event Action EscapePressed = delegate {};
		
		UnixDevice dev;
		MemoryArea buttonMem;
		EventWaitHandle stopPolling = new ManualResetEvent(false);
		const int pollTime = 50;
		const int debounceTime = 10;
		public Buttons ()
		{
			dev = new UnixDevice("/dev/lms_ui");
			buttonMem = dev.MMap(ButtonCount, 0);
			new Thread(ButtonPollThread).Start();
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
		
		ButtonStates GetDebounced()
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
		
		void ButtonPollThread()
		{	
			Thread.CurrentThread.IsBackground = true;
			ButtonStates lastState = GetDebounced();
			while (!stopPolling.WaitOne(pollTime))
			{
				ButtonStates bs = GetDebounced();
				if (bs != lastState)
				{
					ButtonStates pressed = (bs ^ lastState) & (~lastState);
					switch (pressed)
					{
						case ButtonStates.Down:		DownPressed(); break;
						case ButtonStates.Enter:	EnterPressed(); break;
						case ButtonStates.Escape:	EscapePressed(); break;
						case ButtonStates.Left:		LeftPressed(); break;
						case ButtonStates.Right:	RightPressed(); break;
						case ButtonStates.Up:		UpPressed(); break;
					}
					lastState = bs;
				}
				
			}
		}
		
		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			dev.Dispose();
		}
		#endregion
	}
}

