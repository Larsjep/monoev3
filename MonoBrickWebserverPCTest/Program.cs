using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoBrickWebServer;

namespace MonoBrickWebserverPCTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var server = new Webserver(true);
			server.Start(80);
			Console.WriteLine("Press any key to end...");
			Console.ReadKey ();
			server.Stop();
		}
	}
}
