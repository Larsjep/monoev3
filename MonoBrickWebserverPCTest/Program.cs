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
			Webserver.Instance.Start(80, true);
			Console.WriteLine("Press any key to end...");
			Console.ReadKey ();
			Webserver.Instance.Stop();
		}
	}
}
