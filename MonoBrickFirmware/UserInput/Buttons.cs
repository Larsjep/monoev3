using System;
using System.Threading;

namespace MonoBrickFirmware.UserInput
{
  
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
}

