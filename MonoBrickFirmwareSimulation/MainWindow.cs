using Gtk;
using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.FileSystem;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Connections;
using MonoBrickFirmwareSimulation.Mock;
using System.Threading;

public partial class MainWindow: Gtk.Window
{
	private static Thread startupAppThread;
	private static ButtonsMock buttonsMock = new ButtonsMock();
	private static LcdMock lcdMock;
	public static ManualResetEvent LcdReady = new ManualResetEvent(false);

	private static void StartupAppExecution()
	{
		LcdReady.WaitOne ();
		Thread.Sleep (2000);
		StartupApp.MainClass.Main (null);	
	}


	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build();
		lcdDrawingarea.SetSizeRequest(178, 128);
		Buttons.Instance = buttonsMock;
		ProgramManager.Instance = new ProgramManagerMock ();
		WiFiDevice.Device = new WiFiMock ();
		lcdMock = new LcdMock (lcdDrawingarea);
		Lcd.Instance = lcdMock;
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
		buttonsMock.UpPressed ();
	}

	protected void OnUpButtonReleased (object sender, EventArgs e)
	{
		buttonsMock.UpReleased ();
	}

	protected void OnEnterButtonPressed (object sender, EventArgs e)
	{
		buttonsMock.EnterPressed ();
	}

	protected void OnEnterButtonReleased (object sender, EventArgs e)
	{
		buttonsMock.EnterReleased ();
	}

	protected void OnRightButtonPressed (object sender, EventArgs e)
	{
		buttonsMock.RightPressed ();
	}

	protected void OnRightButtonReleased (object sender, EventArgs e)
	{
		buttonsMock.RightReleased ();
	}

	protected void OnLeftButtonPressed (object sender, EventArgs e)
	{
		buttonsMock.LeftPressed ();
	}

	protected void OnLeftButtonReleased (object sender, EventArgs e)
	{
		buttonsMock.LeftReleased ();
	}

	protected void OnDownButtonPressed (object sender, EventArgs e)
	{
		buttonsMock.DownPressed ();
	}

	protected void OnDownButtonReleased (object sender, EventArgs e)
	{
		buttonsMock.DownReleased ();
	}

	protected void OnEscButtonPressed (object sender, EventArgs e)
	{
		buttonsMock.EscPressed ();	
	}

	protected void OnEscButtonReleased (object sender, EventArgs e)
	{
		buttonsMock.EscReleased ();
	}
}
