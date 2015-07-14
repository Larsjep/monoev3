using System;
using Gtk;

namespace EV3MonoBrickSimulator
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			//win.Show ();
			win.ShowAll ();
			MainWindow.LcdReady.Set ();
			Application.Run ();
		}
	}
}
