using System;
using Gtk;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.FileSystem;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Connections;
using MonoBrickFirmware.Settings;
using MonoBrickFirmware.FirmwareUpdate;
using EV3MonoBrickSimulator.Stub;
using EV3MonoBrickSimulator.Settings;
using System.Threading;
using Gdk;
using System.IO;


public partial class MainWindow: Gtk.Window
{
	private static Thread startupAppThread;
	private static LcdStub lcdStub;
	private static ButtonsStub buttonsStub = new ButtonsStub();
	private static WiFiStub wiFiStub = new WiFiStub(); 
	private static SimulatorSettings simulatorSettings = new SimulatorSettings();
	private static ProgramManagerStub programManagerStub = new ProgramManagerStub();
	private static SettingsStub settingsStub = new SettingsStub();
	private static VersionHelperStub versionHelperStub = new VersionHelperStub();
	private static FileSystemWatcher watcher = new FileSystemWatcher();
	private static bool firmwareRunning = false;
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		lcdDrawingarea.SetSizeRequest(178, 128);

		//Set stubs
		lcdStub = new LcdStub (lcdDrawingarea);
		Lcd.Instance = lcdStub;
		Buttons.Instance = buttonsStub;
		ProgramManager.Instance = programManagerStub;
		FirmwareSettings.Instance = settingsStub;
		WiFiDevice.Instance = wiFiStub;
		VersionHelper.Instance = versionHelperStub;


		//Load and apply simulator settings
		simulatorSettings = new SimulatorSettings ();
		simulatorSettings.Load ();
		simulatorSettings.Save ();	
		ApplySettings ();

		//Setup settings changed handler
		watcher.Path = Directory.GetCurrentDirectory();
		watcher.NotifyFilter = NotifyFilters.LastAccess;
		watcher.Filter = simulatorSettings.SettingsFileName;
		watcher.Changed += OnSettingsFileChanged;
		watcher.EnableRaisingEvents = true;

		startupAppThread = new Thread(new ThreadStart(StartupAppExecution));
		startupAppThread.IsBackground = true;
		lcdDrawingarea.ExposeEvent += LcdExposed;
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
		buttonsStub.UpPressed ();
	}

	protected void OnUpEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		upImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_11.png"));
		upImageSmall1.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_16.png"));
		upImageSmall2.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_18.png"));
		buttonsStub.UpReleased ();
	}

	protected void OnDownEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		downImage.Pixbuf = new Pixbuf (global::System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_24_P.png"));
		downImageSmall1.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_21_P.png"));
		downImageSmall2.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_22_P.png"));
		buttonsStub.DownPressed ();
	}

	protected void OnDownEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		downImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_24.png"));
		downImageSmall1.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_21.png"));
		downImageSmall2.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_22.png"));
		buttonsStub.DownReleased ();
	}

	protected void OnEnterEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		enterImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_17_P.png"));
		buttonsStub.EnterPressed ();
	}

	protected void OnEnterEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		enterImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_17.png"));
		buttonsStub.EnterReleased ();
	}

	protected void OnEscEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		escImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_07_P.png"));
		buttonsStub.EscPressed ();
	}

	protected void OnEscEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		escImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_07.png"));
		buttonsStub.EscReleased ();
	}

	protected void OnLeftEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		leftImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_15_P.png"));
		buttonsStub.LeftPressed ();
	}

	protected void OnLeftEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		leftImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_15.png"));
		buttonsStub.LeftReleased ();
	}

	protected void OnRightEventboxButtonPressEvent (object o, ButtonPressEventArgs args)
	{
		rightImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_19_P.png"));
		buttonsStub.RightPressed ();
	}

	protected void OnRightEventboxButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
	{
		rightImage.Pixbuf = new global::Gdk.Pixbuf (global::System.IO.Path.Combine (global::System.AppDomain.CurrentDomain.BaseDirectory, "./Images/EV3-multibrick_19.png"));
		buttonsStub.RightReleased ();
	}

	private static void OnSettingsFileChanged(object sender, FileSystemEventArgs e)
	{
		ApplySettings ();	
	}

	private static void ApplySettings()
	{
		simulatorSettings.Load ();
		wiFiStub.TurnOnTimeMs = simulatorSettings.WiFiSettings.TurnOnTimeMs;
		wiFiStub.TurnOffTimeMs = simulatorSettings.WiFiSettings.TurnOffTimeMs;

		versionHelperStub.ImageVersion = simulatorSettings.VersionSettings.ImageVersion;
		versionHelperStub.Repository = simulatorSettings.VersionSettings.Repository;
		versionHelperStub.AddInVersion = simulatorSettings.VersionSettings.AddInVersion;

		programManagerStub.AOTCompileTimeMs = simulatorSettings.ProgramManagerSettings.AotCompileTimeMs;

	}

	private static void LcdExposed(object o, ExposeEventArgs args)
	{
		if (firmwareRunning) 
		{
			Lcd.Update ();
		} 
		else 
		{
			startupAppThread.Start ();	
		}
	}

	private static void StartupAppExecution()
	{
		firmwareRunning = true;
		Thread.Sleep (simulatorSettings.BootSettings.ExecutionDelay);
		StartupApp.MainClass.Main (null);
	}


}
