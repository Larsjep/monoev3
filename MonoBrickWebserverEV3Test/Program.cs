using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoBrickWebServer;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Display.Dialogs;

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

		static void Main(string[] args)
		{
			StepContainer step = new StepContainer( StartWebserver, "Starting", "Failed to start");
			Dialog dialog = new ProgressDialog("Webserver", step); 
			dialog.Show();
			dialog = new InfoDialog("Press enter to stop", true, "Webserver");
			dialog.Show();
		}
	}
}
