using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoBrickWebServer;
using System.Net;

namespace MonoBrickWebserverPCTest
{
	class Program
	{
		static bool StartWebserver ()
		{
			return Webserver.Instance.Start(8080, 3 * 60000);
		}

		static bool LoadPage (string url)
		{
			Console.WriteLine("Loading url: " +  url);
			WebClient client = new WebClient();
			string content = client.DownloadString("http://127.0.0.1:8080" +  url);
			Console.WriteLine(content);
			return true;
		}

		static void Main (string[] args)
		{
			Webserver.Instance.Start (8080, -1, true);
			var urls = Webserver.Instance.Urls;
			foreach (var s in urls) 
			{
				LoadPage(s);
			}


			Console.WriteLine("Press any key to end...");
			Console.ReadKey ();
			Webserver.Instance.Stop();
		}
	}
}
