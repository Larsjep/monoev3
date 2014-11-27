using System;
using System.Threading;
using MonoBrickFirmware.Movement;
using MonoBrickWebServer.Models;
using MonoBrickWebServer.Modules;
using Nancy.Hosting.Self;
using System.Linq;

namespace MonoBrickWebServer
{
	public class Webserver
	{

		private ManualResetEvent terminateServer = new ManualResetEvent(false);
		private ManualResetEvent serverStarted = new ManualResetEvent(false);
		private NancyHost nancyHost = null;
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

		private void MainThread ()
		{
			running = true;
			try 
			{
				nancyHost = new NancyHost (new Uri ("http://localhost:" + Port + "/"));
				nancyHost.Start ();
				Console.WriteLine("Nancy webserver listening at " + Port);
				serverStarted.Set ();
			} 
			catch(Exception e) {Console.WriteLine(e.Message);Console.WriteLine(e);}
			terminateServer.WaitOne ();
			Console.WriteLine("Nancy webserver stopped ");
			running = false;
			try 
			{
				nancyHost.Stop ();
			} 
			catch{}
		}


		public bool Start (int port, int timeout = Timeout.Infinite, bool useDummyEV3 = false)
		{
			if (IsRunning) 
			{
				Stop ();
			}
			Port = port;
			serverStarted.Reset ();
			EV3Module.EV3 = new EV3Model (useDummyEV3);
			serverThread.Start ();
			Console.WriteLine("Starting web server");
			if (!serverStarted.WaitOne (timeout)) 
			{
				Console.WriteLine("Start timeout");
				if(nancyHost != null)
					nancyHost.Stop();
				Stop();	
			}
			System.Threading.Thread.Sleep(500);
			return IsRunning;
		}

		public string[] Urls{ 
			get 
			{
				return EV3Module.DokumentationList.Select(x => x.URL).ToArray();
			}
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

