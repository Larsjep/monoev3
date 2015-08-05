using System;
using System.IO;
using System.Linq;
using System.Threading;
using Gdk;
using Gtk;

using MonoBrickFirmware.Display;
using MonoBrickFirmware.FileSystem;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Connections;
using MonoBrickFirmware.Settings;
using MonoBrickFirmware.FirmwareUpdate;
using MonoBrickFirmware.Device;

using EV3MonoBrickSimulator.Stub;
using EV3MonoBrickSimulator.Settings;
using EV3MonoBrickSimulator;
using System.Reflection;


public partial class MainWindow: Gtk.Window
{
	private static Thread startupAppThread;
	private static LcdStub lcdStub;
	private static ButtonsStub buttonsStub = new ButtonsStub();
	private static WiFiStub wiFiStub = new WiFiStub(); 
	private static SimulatorSettings simulatorSettings = new SimulatorSettings();
	private static ProgramManagerStub programManagerStub = new ProgramManagerStub();
	private static EV3FirmwareSettings firmwareSettings = new EV3FirmwareSettings ("FirmwareSettings.xml");
	private static UpdateHelperStub updateHelperStub = new UpdateHelperStub();
	private static FileSystemWatcher watcher = new FileSystemWatcher();
	private static bool firmwareRunning = false;
  private static MethodInfo killMethod;
	private static BrickStub brickStub = null;
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		lcdDrawingarea.SetSizeRequest(178, 128);

		//Set stubs
		lcdStub = new LcdStub (lcdDrawingarea);
		brickStub = new BrickStub ();
		brickStub.OnShutDown += OnShutDown;
		Lcd.Instance = lcdStub;

		Buttons.Instance = buttonsStub;
		ProgramManager.Instance = programManagerStub;
		FirmwareSettings.Instance = firmwareSettings;
		WiFiDevice.Instance = wiFiStub;
		UpdateHelper.Instance = updateHelperStub;
		Brick.Instance = brickStub;

		//Load and apply simulator settings
		simulatorSettings = new SimulatorSettings ();
		simulatorSettings.Load ();
		simulatorSettings.Save ();	
		ApplySettings ();

		//Setup settings changed handler
		watcher.Path = Directory.GetCurrentDirectory();
		watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite| NotifyFilters.CreationTime;
		watcher.Filter = simulatorSettings.SettingsFileName;
		watcher.Changed += OnSettingsFileChanged;
		watcher.EnableRaisingEvents = true;

		lcdDrawingarea.ExposeEvent += LcdExposed;
	}

	void OnShutDown ()
	{
		KillFirmware ();
		StartFirmware ();
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

		updateHelperStub.ImageVersion = simulatorSettings.VersionSettings.ImageVersion;
		updateHelperStub.Repository = simulatorSettings.VersionSettings.Repository;
		updateHelperStub.AddInVersion = simulatorSettings.VersionSettings.AddInVersion;

		programManagerStub.AOTCompileTimeMs = simulatorSettings.ProgramManagerSettings.AotCompileTimeMs;

		brickStub.TurnOffTimeMs = simulatorSettings.BootSettings.TurnOffDelay;

	}

	private static void LcdExposed(object o, ExposeEventArgs args)
	{
		if (firmwareRunning) 
		{
			Lcd.Update ();
		} 
		else 
		{
			StartFirmware ();
		}
	}

	private static void StartFirmware()
	{
		startupAppThread = new Thread(new ThreadStart(StartupAppExecution));
		startupAppThread.IsBackground = true;
		startupAppThread.Start ();	

	}

	private static void KillFirmware()
	{
	  killMethod.Invoke(null, null);
	}

	private static void StartupAppExecution()
	{
		firmwareRunning = true;
		Thread.Sleep (simulatorSettings.BootSettings.ExecutionDelay);
		string startUpAppPath = System.IO.Path.Combine (simulatorSettings.BootSettings.StartUpDir, "StartupApp.exe");
		if(!Directory.Exists(simulatorSettings.BootSettings.StartUpDir))
		{
			startUpAppPath = System.IO.Path.Combine (Directory.GetCurrentDirectory(), "StartupApp.exe");
			simulatorSettings.BootSettings.StartUpDir = Directory.GetCurrentDirectory ();
			simulatorSettings.Save ();
		}
    
    var test = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(System.IO.Path.Combine (Directory.GetCurrentDirectory(), "Test.exe"), "StartupApp.MainClass");
		string arg = null;
	  var methods = test.GetType().GetMethods();
	  MethodInfo mainMethod = null; 
    foreach (var method in methods)
	  {
	    if (method.Name == "Main")
	    {
	      mainMethod = method;
	    }
	    if (method.Name == "Kill")
	    {
	      killMethod = method;
	    }
	  }
    mainMethod.Invoke(null, new object[] { arg });
    firmwareRunning = false;


    /*starUpApp.GetType ().GetMethod ("Main");
		method.Invoke(null,new object[]{arg});
		firmwareRunning = false;*/
	}


}
