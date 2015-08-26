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

		static void StopWebserver ()
		{
			Webserver.Instance.Stop();
		}

		static void Main (string[] args)
		{
			var step = new StepContainer (StartWebserver, "Starting server", "Failed to start");
			Dialog dialog = new ProgressDialog("Webserver", step);
			if (dialog.Show ()) {
				dialog = new InfoDialog ("Press enter to stop", "Webserver");
				dialog.Show ();
				StopWebserver ();
			}
			
		}
	}
}
