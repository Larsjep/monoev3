using System;
using System.Threading;
using MonoBrickFirmware.Movement;
using MonoBrickWebServer.Models;
using MonoBrickWebServer.Modules;
using Nancy.Hosting.Self;

namespace MonoBrickWebServer
{
	public class Webserver
	{

		private ManualResetEvent terminateServer = new ManualResetEvent(false);
		private ManualResetEvent serverStarted = new ManualResetEvent(false);
		private Thread serverThread;
		private bool running = false;
		private static Webserver instance = new Webserver();
		public static Webserver Instance
		{
			get 
			{
				if (instance == null)
					instance = new Webserver ();
				return instance;
			}
		}

		private Webserver ()
		{
			serverThread = new Thread(MainThread);
		}

		private void MainThread()
		{
			running = true;
			var nancyHost = new NancyHost(new Uri("http://127.0.0.1:" + Port + "/"));
			nancyHost.Start();
			serverStarted.Set();
			Console.WriteLine("Nancy now listening at port: " + Port);
			terminateServer.WaitOne();
			nancyHost.Stop ();
			running = false;
		}


		public void Start (int port, bool useDummyEV3 = false)
		{
			if (IsRunning) 
			{
				Stop();
			}
			Port = port;
			serverStarted.Reset();
			EV3Module.EV3 = new EV3Model(useDummyEV3);
			serverThread.Start();
			serverStarted.WaitOne();

		}

		public void Stop()
		{
		  terminateServer.Set();
		  serverThread.Join();
		}

		public int Port{ get; private set;}

		public bool IsRunning 
		{
			get { return running; }
		}
	}
}

