using System;
using System.Reflection;
using System.Linq;
using System.Net;
using System.IO;

namespace MonoBrickWebserverDynamicTest
{
	public class Webserver
	{
		
		private static Webserver instance = new Webserver();
		private string DllPath = System.IO.Directory.GetCurrentDirectory() + @"/MonoBrickWebServer.dll";
		private Type webServerType = null;  
		private object webServerInstance = null;
		private int port;
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
			IsAssemblyLoaded = false;
		}

		public bool LoadAssembly ()
		{
			if (!IsAssemblyLoaded) 
			{
				try {
					webServerType = Assembly.LoadFile (DllPath).GetExportedTypes ().First (type => type.FullName.Contains ("Webserver"));
					webServerInstance = webServerType.GetProperty("Instance").GetValue(null,null);
					IsAssemblyLoaded = (webServerType != null && webServerInstance != null);
				} 
				catch 
				{

				}
			}
			return IsAssemblyLoaded;
		}

		public bool LoadPage()
		{
			if(!IsRunning)
				return false;
			int attemps = 0;
			bool loaded = false;
			while(attemps < 2 && !loaded){
				try 
				{
					port = 59;
					HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(@"http://127.0.0.1:"+ port);
					myRequest.Method = "GET";
					WebResponse myResponse = myRequest.GetResponse();
					StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
					string result = sr.ReadToEnd();
					Console.WriteLine(result);
					sr.Close();
					myResponse.Close();
					loaded = true;
				} 
				catch(Exception e) 
				{
					Console.WriteLine(e.StackTrace);
				}
				attemps++;
			}
			return loaded;
		}

		public void Start (int port, bool dummyMode = false)
		{
			this.port = port;
			if(IsAssemblyLoaded)
				webServerType.InvokeMember ("Start", BindingFlags.OptionalParamBinding | BindingFlags.Default | BindingFlags.InvokeMethod, null, webServerInstance, new object[]{port, dummyMode});
		}

		public void Stop ()
		{
			if(IsAssemblyLoaded)
				webServerType.InvokeMember ("Stop", BindingFlags.Default | BindingFlags.InvokeMethod, null, webServerInstance, new object[]{});
		}

		public bool IsAssemblyLoaded{get; private set;}

		public bool IsRunning
		{ 
			get 
			{ 
				if(!IsAssemblyLoaded)
					return false;
				return (bool)webServerInstance.GetType().GetProperty("IsRunning").GetValue(webServerInstance,null);
			}
		}

	}


	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine("Load Assembly: " + Webserver.Instance.IsAssemblyLoaded);
			Webserver.Instance.LoadAssembly();
			Console.WriteLine("Assembly Loaded: " + Webserver.Instance.IsAssemblyLoaded);
			Console.WriteLine ("Start webserver");
			Webserver.Instance.Start(8080, true);
			Console.WriteLine ("Webpage Loaded: " + Webserver.Instance.LoadPage());
			Console.WriteLine("Server Is Running : " + Webserver.Instance.IsRunning);
			Console.WriteLine("Press enter to stop : " + Webserver.Instance.IsRunning);
			Console.ReadLine();
			Webserver.Instance.Stop();
		}
	}
}
