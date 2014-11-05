using System;
using System.Reflection;
namespace MonoBrickWebserverDynamicTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var dllPath = System.IO.Directory.GetCurrentDirectory() + @"\MonoBrickWebServer.dll";
			Console.WriteLine (dllPath);
			var DLL = Assembly.LoadFile(dllPath);
			object webServer = null;
			foreach(Type type in DLL.GetExportedTypes())
			{
				if (type.IsClass && type.FullName.Contains("webserver")) 
				{
					webServer = Activator.CreateInstance (type);
				}
			}

			webServer
		}
	}
}
