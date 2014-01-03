using System;
using MonoBrickFirmware.Sound;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Display;
namespace SoundExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Buttons btns = new Buttons ();
			var speaker = new Speaker ();
			//LcdConsole.WriteLine ("Beep");
			//speaker.Beep (2000, 20);
			
			speaker.PlayTone (200, 300, 0);
			
			for (int i = 0; i < 20; i++) {
				LcdConsole.WriteLine (i.ToString());
				speaker.PlayTone (1000, 300, i);
				System.Threading.Thread.Sleep(1000);
			}
			
				
			
			
			LcdConsole.WriteLine("Press enter to terminate");
			btns.GetKeypress();
		}
	}
}
