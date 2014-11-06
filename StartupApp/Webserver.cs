using System;
using System.Reflection;
using System.Linq;

namespace StartupApp
{
	public class Webserver
	{
		
		private static Webserver instance = new Webserver();
		private string DllPath = System.IO.Directory.GetCurrentDirectory() + @"/MonoBrickWebServer.dll";
		private Type webServerType = null;  
		private object webServerInstance = null;
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

		public void Start (int port, bool dummyMode = false)
		{
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
}

