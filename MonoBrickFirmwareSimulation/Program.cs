using Gtk;
using System;
using System.Threading;

namespace MonoBrickFirmwareSimulation
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			win.LcdReady.Set ();
			Application.Run ();
		}
	}
}
