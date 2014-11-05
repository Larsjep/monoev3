using System;
using System.Reflection;
using System.Linq;
namespace MonoBrickWebserverDynamicTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			int port = 80;
			var dllPath = System.IO.Directory.GetCurrentDirectory() + @"\MonoBrickWebServer.dll";
			Console.WriteLine (dllPath);
			var DLL = Assembly.LoadFile(dllPath);
			Type webServerType = Assembly.LoadFile (dllPath).GetExportedTypes ().First(type => type.FullName.Contains("Webserver"));  
			object webServerInstance = webServerType.GetProperty("Instance").GetValue(null);;
			webServerType.InvokeMember ("Start", BindingFlags.Default | BindingFlags.InvokeMethod, null, webServerInstance, new object[]{port, (bool) true});
			Console.WriteLine ("Started web server on port " + port);
			Console.ReadLine();
			webServerType.InvokeMember ("Stop", BindingFlags.Default | BindingFlags.InvokeMethod, null, webServerInstance, new object[]{});
		}
	}
}
