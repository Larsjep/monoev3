using Gtk;
using System;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.FileSystem;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Connections;
using MonoBrickFirmware.Settings;
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
		FirmwareSettings.Settings = new SettingsMock ();
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
		SpawnThread (buttonsMock.UpPressed);
	}

	protected void OnUpButtonReleased (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.UpReleased);
	}

	protected void OnEnterButtonPressed (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.EnterPressed);
	}

	protected void OnEnterButtonReleased (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.EnterReleased);
	}

	protected void OnRightButtonPressed (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.RightPressed);
	}

	protected void OnRightButtonReleased (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.RightReleased);
	}

	protected void OnLeftButtonPressed (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.LeftPressed);
	}

	protected void OnLeftButtonReleased (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.LeftReleased);
	}

	protected void OnDownButtonPressed (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.DownPressed);
	}

	protected void OnDownButtonReleased (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.DownReleased);
	}

	protected void OnEscButtonPressed (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.EscPressed);	
	}

	protected void OnEscButtonReleased (object sender, EventArgs e)
	{
		SpawnThread (buttonsMock.EscReleased);
	}


	private Semaphore threadMutex = new Semaphore(1, 1); //Ensure that  only one operation is started at the same time
	private void SpawnThread(System.Action action)
	{
		Thread t = new Thread(
			new ThreadStart(
				delegate()
				{
					if (threadMutex.WaitOne())
					{
						try
						{	
							action();	
						}
						catch
						{

						}
						finally
						{
							threadMutex.Release();
						}
					}
				}
			));
		t.IsBackground = true;
		t.Priority = ThreadPriority.AboveNormal;
		t.Start();
	}

}
