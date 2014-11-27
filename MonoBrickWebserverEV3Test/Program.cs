using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoBrickWebServer;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Display.Dialogs;
using System.Net;
using System.IO;

namespace MonoBrickWebserverEV3Test
{
	class Program
	{
		static bool StartWebserver ()
		{
			return Webserver.Instance.Start(8080, 3 * 60000);
		}

		static bool LoadPage (string url)
		{
			int attemps = 0;
			bool loaded = false;
			while(attemps < 2 && !loaded){
				try 
				{
					HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(@"http://127.0.0.1:8080" + url);
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

		static void StopWebserver ()
		{
			Webserver.Instance.Stop();
		}

		static void Main (string[] args)
		{
			IStep startStep = new StepContainer (StartWebserver, "Starting", "Failed to start");
			Dialog dialog = new ProgressDialog("Starting", startStep);
			if(dialog.Show())
			{ 
				List<IStep> steps = new List<IStep> ();
				/*Console.WriteLine("Adding urls to list");
				foreach (var url in Webserver.Instance.Urls) 
				{
					Console.WriteLine("Adding: " +  url);
					steps.Add (new StepContainer (delegate() {return LoadPage(url);}, "url", "Load failed"));
				}
				dialog = new StepDialog("Webserver", steps);
				dialog.Show();*/
				dialog = new InfoDialog("Press enter to stop", true, "Webserver");
				dialog.Show();
				StopWebserver();
			}
		}
	}
}
