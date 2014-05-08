using System;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Tools;
using System.Threading;

namespace MonoBrickFirmware.UserInput
{
	public class Buttons
	{
		private static readonly Buttons instance = new Buttons();
		
		[Flags]
		public enum ButtonStates
		{
			None = 0x00,
			Up = 0x01,
			Enter = 0x02,
			Down = 0x04,
			Right = 0x08,
			Left = 0x10,
			Escape = 0x20,
			All = 0xff,
		}

		public const int ButtonCount = 6;
		UnixDevice dev;
		MemoryArea buttonMem;
		const int debounceTime = 10;
		const int pollTime = 50;

		private Buttons ()
		{
			dev = new UnixDevice ("/dev/lms_ui");
			buttonMem = dev.MMap (ButtonCount, 0);			
		}
		
		public static Buttons Instance
		{
			get 
	      	{
				return instance; 
	      	}	
		}
		
		
		ButtonStates ReadStates()
		{
			ButtonStates bs = ButtonStates.None;
			byte[] buttonData = buttonMem.Read ();
			int bitMask = 1;
			foreach (byte butState in buttonData) {
				if (butState != 0)
					bs |= (ButtonStates)bitMask;
				bitMask = bitMask << 1;
			}
			return bs;
		}

		internal ButtonStates GetDebounced ()
		{
			
			ButtonStates s2 = ButtonStates.None;
			for (;;) {
				ButtonStates s1 = ReadStates ();
				if (s1 == s2)
					return s1;
				Thread.Sleep (debounceTime);
				s2 = ReadStates ();
			}
		}

		public ButtonStates GetStates ()
		{
			return GetDebounced ();
		}

		public void WaitForKeyRelease (CancellationToken token)
		{
			ButtonStates bs = GetDebounced ();
			do {				
				Thread.Sleep (pollTime);
				bs = GetDebounced ();
			} while (bs != ButtonStates.None && !token.IsCancellationRequested);
		}
		
		public void WaitForKeyRelease ()
		{
			WaitForKeyRelease(new CancellationToken(false));
		}

		public ButtonStates GetKeypress (CancellationToken token)
		{
			ButtonStates bs = GetDebounced ();
			while (bs != ButtonStates.None && !token.IsCancellationRequested) {
				bs = GetDebounced ();
			}
			do {				
				Thread.Sleep (pollTime);
				bs = GetDebounced ();
			} while (bs == ButtonStates.None && !token.IsCancellationRequested);
			return bs;
		}
		
		public ButtonStates GetKeypress ()
		{
			return GetKeypress(new CancellationToken(false));
		}
		

		public void LedPattern (int pattern)
		{
			byte[] cmd = new byte[2];
			cmd [0] = (byte)('0' + pattern);
			dev.Write (cmd);
		}

	}

}

