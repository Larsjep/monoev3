using System;
using MonoBrickFirmware.Sound;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Display;
using System.Threading;
namespace SoundExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string soundFileName = "/home/root/apps/SoundTest.wav";
			ManualResetEvent terminateProgram = new ManualResetEvent(false);
			var speaker = new Speaker ();
			ButtonEvents buts = new ButtonEvents ();
			LcdConsole.WriteLine("Up beep");
			LcdConsole.WriteLine("Down buzz");
			LcdConsole.WriteLine("Enter play soundfile");
			LcdConsole.WriteLine("Esc. terminate");
			buts.EscapePressed += () => { 
				terminateProgram.Set();
			};
			buts.UpPressed += () => {
				LcdConsole.WriteLine("Beep");
				speaker.Beep();
			};
			buts.DownPressed += () => { 
				LcdConsole.WriteLine("Buzz");
				speaker.Buzz();
			};
			buts.EnterPressed += () => { 
				LcdConsole.WriteLine("Play sound file");
				try{
					speaker.PlaySoundFile(soundFileName);
				}
				catch(Exception e)
				{
					LcdConsole.WriteLine("Failed to play " + soundFileName);
					LcdConsole.WriteLine("Exception" + e.Message);
					LcdConsole.WriteLine("Stack trace " + e.StackTrace);
					
				
				}
			};   
			terminateProgram.WaitOne();
		}
	}
}
