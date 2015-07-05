using Gtk;
using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmwareSimulation.Mock;
using System.Threading;

public partial class MainWindow: Gtk.Window
{
	private Thread startupAppThread;
	public ManualResetEvent LcdReady = new ManualResetEvent(false);
	private void StartupAppExecution()
	{
		lcdDrawingarea.SetSizeRequest(178, 128);
		Lcd.Instance = new LcdMock (lcdDrawingarea, 4);
		StartupApp.MainClass.Main (null);	
	}


	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build();
		startupAppThread = new Thread(new ThreadStart(StartupAppExecution));
		startupAppThread.IsBackground = true;
		startupAppThread.Start ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnUpButtonPressed (object sender, EventArgs e)
	{
		Rectangle textRect = new Rectangle (new Point (0, Lcd.Height - (int)Font.SmallFont.maxHeight - 2), new Point (Lcd.Width, Lcd.Height - 2));
		Lcd.WriteTextBox (Font.SmallFont, textRect, "Initializing...", true, Lcd.Alignment.Center);
		Lcd.Update ();

	}
}
