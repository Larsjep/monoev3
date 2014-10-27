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
		private Thread serverThread;
		private bool running = false;

		public Webserver (bool useDummy = false)
		{
			serverThread = new Thread(MainThread);
			EV3Module.EV3 = new EV3Model (useDummy);
		}

		private void MainThread()
		{
			running = true;
			var nancyHost = new NancyHost(new Uri("http://127.0.0.1:" + Port + "/"));
			nancyHost.Start();
			Console.WriteLine("Nancy now listening at port: " + Port);
			terminateServer.WaitOne();
			nancyHost.Stop ();
			running = false;
		}


		public void Start(int port)
		{
			Port = port;
			serverThread.Start();
		}

		public void Stop()
		{
		  terminateServer.Set();
		  serverThread.Join();
		}

		public int Port{ get; private set;}

		public bool Running 
		{
			get { return running; }
		}
	}
}

