using System;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.Services
{
	public class WebServer
	{
		private static string webserPath = "/home/root/webserver";
		private static string xspName = "xsp4.exe";
		private static string xspPath = "/usr/local/lib/mono/";
		public WebServer ()
		{
		}
		
		private static bool DoesXSPExsistsInWebServerPath ()
		{
			return true;
		}
		
		private static void CopyXSPToWebServerPath ()
		{
		
		}
		
		public static bool Start()
		{
			return true;
		}
		
		public static void Stop ()
		{
			
		}
		
		public static bool IsRunning ()
		{
			return true;
		}
	}
}

