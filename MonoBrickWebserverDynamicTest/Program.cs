using System;
using System.Reflection;
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
			object webServerInstance = null;
			Type webServerType = null;
			foreach(Type type in DLL.GetExportedTypes())
			{
				if (type.IsClass && type.FullName.Contains("Webserver")) 
				{
					var props = type.GetProperties();
					webServerType = type;
					FieldInfo field = type.GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
					webServerInstance = field.GetValue(null);

				}
			}
			if(webServerInstance == null)
				Console.WriteLine ("Is null");
			webServerType.InvokeMember ("Start", BindingFlags.Default | BindingFlags.InvokeMethod, null, webServerInstance, new object[]{port, (bool) true});
			Console.WriteLine ("Started web server on port " + port);
			Console.ReadLine();
			webServerType.InvokeMember ("Stop", BindingFlags.Default | BindingFlags.InvokeMethod, null, webServerInstance, new object[]{});
		}
	}
}
