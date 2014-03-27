using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net;
using MonoBrickFirmware.Native;
using System.Net.Sockets;
namespace MonoBrickFirmware.Services
{
	public class WebServer : IDisposable
	{
		private const string logDir = "/var/log/lighttpd/";
		private const string wwwDir = "/www";
		
		private const string fastCGIName = "fastcgi-mono-server4.exe";
		private const string fastCGIPath = "/usr/local/bin/";
		private const string fastCGIArgs = "/socket=tcp:9000 /address=127.0.0.1 /applications=/:.,/peanuts: "+ wwwDir + "/logfile=" +logDir + "fastcgi.log /verbose &";

		private const string lighttpdName = "lighttpd";
		private const string lighttpdPath = "/usr/local/sbin/";
		private const string lighttpdConf = "/etc/lighttpd/lighttpd.conf";
		
		private bool StartFastCGI ()
		{
			return ProcessHelper.RunAndWaitForProcess("mono", fastCGIPath + fastCGIName + " " + fastCGIArgs) == 0;
		}
		
		private void StopFastCGI ()
		{
			ProcessHelper.KillProcess(fastCGIName);
		}
		
		private bool StartLighttpd ()
		{
			ProcessHelper.RunAndWaitForProcess("adduser", "lighttpd");
			ProcessHelper.RunAndWaitForProcess("mkdir", logDir);
			ProcessHelper.RunAndWaitForProcess("chown", "_R lighttpd:lighttpd " +  logDir);
			return ProcessHelper.RunAndWaitForProcess(lighttpdPath+lighttpdName, "-f " + lighttpdConf) == 0;
		}
		
		private void StopLighttpd ()
		{
			ProcessHelper.KillProcess(lighttpdName);	
		}
		
		private bool LoadPage ()
		{
			int attemps = 0;
			bool loaded = false;
			while(attemps < 2 && !loaded){
				try {
					/*Console.WriteLine("Load webpage");
					WebClient client = new WebClient();
					client.DownloadString("http://127.0.0.1"+":"+port);
					loaded = true;*/
					/*HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(@"http://127.0.0.1:"+ port);
					myRequest.Method = "GET";
					WebResponse myResponse = myRequest.GetResponse();
					StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
					string result = sr.ReadToEnd();
					sr.Close();
					myResponse.Close();
					loaded = true;*/
				} 
				catch(Exception e) 
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
				}
				attemps++;
			}
			return true;
			//return loaded;
		}
		
		public Action StartingServer = delegate {};
		public Action LoadingPage = delegate {};
		
		public WebServer ()
		{

		}
		
		public bool Start ()
		{
			bool running = true;
			if (!IsRunning()) 
			{
				running = false;
				try{
					StartingServer();
					if(StartFastCGI() && StartLighttpd())
					{
						LoadingPage();
						if(LoadPage())
						{
							running = true;
						}
						else
						{
							Stop();
						}
						
					}
				}
				catch
				{
					Stop();
					running = false;
				}
			}
			return running;
		}
		
		public bool Restart ()
		{
			Stop();
			Thread.Sleep(1000);
			return Start();
		}
		
		public void Stop ()
		{
			if (IsRunning ()) {
				StopFastCGI();
				StopLighttpd();
			}
		}
		
		public static bool IsRunning ()
		{
			return ProcessHelper.IsProcessRunning(lighttpdName) && ProcessHelper.IsProcessRunning(fastCGIName);
		}
		
		public void Dispose()
		{			
			Stop();
			GC.SuppressFinalize(this);	
		}
		
		~WebServer()
		{
			Stop();
		}
	}
}

