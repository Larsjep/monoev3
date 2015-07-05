using System;
using MonoBrickFirmware.Native;
using MonoBrickFirmware.Tools;
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

  public static class Buttons
  {
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

    static Buttons()
    {
		try
		{
			Instance = new EV3Buttons();
		}
		catch
		{
			Instance = null;		
		}
    }

    public static IButtons Instance { get; set;}

    public static ButtonStates GetStates()
    {
      return Instance.GetStates();
    }

    public static void WaitForKeyRelease(CancellationToken token)
    {
      Instance.WaitForKeyRelease(token);
    }

    public static void WaitForKeyRelease()
    {
      Instance.WaitForKeyRelease();
    }

    public static ButtonStates GetKeypress(CancellationToken token)
    {
      return Instance.GetKeypress(token);
    }

    public static ButtonStates GetKeypress()
    {
      return Instance.GetKeypress();
    }

    public static void LedPattern(int pattern)
    {
      Instance.LedPattern(pattern);
    }

  }

  public class EV3Buttons : IButtons
  {
		public const int ButtonCount = 6;
		UnixDevice dev;
		MemoryArea buttonMem;
		const int debounceTime = 10;
		const int pollTime = 50;

    	public EV3Buttons()
		{
			dev = new UnixDevice ("/dev/lms_ui");
			buttonMem = dev.MMap (ButtonCount, 0);			
		}

		public Buttons.ButtonStates GetStates ()
		{
			return GetDebounced ();
		}

		public void WaitForKeyRelease (CancellationToken token)
		{
			var bs = GetDebounced ();
			do {				
				Thread.Sleep (pollTime);
				bs = GetDebounced ();
			} while (bs != Buttons.ButtonStates.None && !token.IsCancellationRequested);
		}
		
		public void WaitForKeyRelease ()
		{
			WaitForKeyRelease(new CancellationToken(false));
		}

    	public Buttons.ButtonStates GetKeypress(CancellationToken token)
		{
			Buttons.ButtonStates bs = GetDebounced();
			while (bs != Buttons.ButtonStates.None && !token.IsCancellationRequested)
			{
				bs = GetDebounced ();
			}
			do {				
				Thread.Sleep (pollTime);
				bs = GetDebounced ();
			} while (bs == Buttons.ButtonStates.None && !token.IsCancellationRequested);
			return bs;
		}

    	public Buttons.ButtonStates GetKeypress()
		{
			return GetKeypress(new CancellationToken(false));
		}
		

		public void LedPattern (int pattern)
		{
			byte[] cmd = new byte[2];
			cmd [0] = (byte)('0' + pattern);
			dev.Write (cmd);
		}

		private Buttons.ButtonStates ReadStates()
		{
			var bs = Buttons.ButtonStates.None;
			byte[] buttonData = buttonMem.Read ();
			int bitMask = 1;
			foreach (byte butState in buttonData) {
				if (butState != 0)
					bs |= (Buttons.ButtonStates)bitMask;
				bitMask = bitMask << 1;
			}
			return bs;
		}

		private Buttons.ButtonStates GetDebounced ()
		{

			Buttons.ButtonStates s2 = Buttons.ButtonStates.None;
			for (;;) 
			{
				Buttons.ButtonStates s1 = ReadStates();
				if (s1 == s2)
					return s1;
				Thread.Sleep (debounceTime);
				s2 = ReadStates ();
			}
		}

	}

}

