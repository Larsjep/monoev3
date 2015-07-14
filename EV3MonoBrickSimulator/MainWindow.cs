using System;
using Gtk;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.FileSystem;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Connections;
using MonoBrickFirmware.Settings;
using EV3MonoBrickSimulator.Stub;
using System.Threading;

public partial class MainWindow: Gtk.Window
{
	private static Thread startupAppThread;
	private static ButtonsStub buttonsMock = new ButtonsStub();
	private static LcdStub lcdMock;
	public static ManualResetEvent LcdReady = new ManualResetEvent(false);

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		lcdDrawingarea.SetSizeRequest(178, 128);
		Buttons.Instance = buttonsMock;
		ProgramManager.Instance = new ProgramManagerStub ();
		WiFiDevice.Device = new WiFiStub ();
		FirmwareSettings.Settings = new SettingsStub ();
		lcdMock = new LcdStub (lcdDrawingarea);
		Lcd.Instance = lcdMock;
		startupAppThread = new Thread(new ThreadStart(StartupAppExecution));
		startupAppThread.IsBackground = true;
		startupAppThread.Start ();
	}

	private static void StartupAppExecution()
	{
		LcdReady.WaitOne ();
		Thread.Sleep (2000);
		StartupApp.MainClass.Main (null);	
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnUpEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		upImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_11_P.png"));
		upImageSmall1.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_16_P.png"));
		upImageSmall2.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_18_P.png"));
		buttonsMock.UpPressed ();
	}

	protected void OnUpEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		upImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_11.png"));
		upImageSmall1.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_16.png"));
		upImageSmall2.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_18.png"));
		buttonsMock.UpReleased ();
	}

	protected void OnDownEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		downImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_24_P.png"));
		downImageSmall1.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_21_P.png"));
		downImageSmall2.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_22_P.png"));
		buttonsMock.DownPressed ();		
	}

	protected void OnDownEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		downImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_24.png"));
		downImageSmall1.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_21.png"));
		downImageSmall2.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_22.png"));
		buttonsMock.DownReleased ();
	}

	protected void OnEnterEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		enterImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_17_P.png"));
		buttonsMock.EnterPressed ();
	}

	protected void OnEnterEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		enterImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_17.png"));
		buttonsMock.EnterReleased ();
	}

	protected void OnEscEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		escImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_07_P.png"));
		buttonsMock.EscPressed ();
	}

	protected void OnEscEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		escImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_07.png"));
		buttonsMock.EscReleased ();
	}

	protected void OnLeftEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		leftImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_15_P.png"));
		buttonsMock.LeftPressed ();
	}

	protected void OnLeftEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		leftImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_15.png"));
		buttonsMock.LeftReleased ();
	}

	protected void OnRightEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		rightImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_19_P.png"));
		buttonsMock.RightPressed ();
	}

	protected void OnRightEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		rightImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_19.png"));
		buttonsMock.RightReleased ();
	}
}
