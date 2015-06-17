using MonoBrickFirmware.Display.Menus;
using MonoBrickFirmware.Display.Dialogs;

namespace StartupApp
{
	public class ItemWithWebserver : IChildItem
	{
		public ItemWithWebserver ()
		{
		}

		public void OnEnterPressed ()
		{
			throw new System.NotImplementedException ();
		}

		public void OnLeftPressed ()
		{
			throw new System.NotImplementedException ();
		}

		public void OnRightPressed ()
		{
			throw new System.NotImplementedException ();
		}

		public void OnUpPressed ()
		{
			throw new System.NotImplementedException ();
		}

		public void OnDownPressed ()
		{
			throw new System.NotImplementedException ();
		}

		public void OnEscPressed ()
		{
			throw new System.NotImplementedException ();
		}

		public void OnDrawTitle (MonoBrickFirmware.Display.Font font, MonoBrickFirmware.Display.Rectangle rectangle, bool selected)
		{
			throw new System.NotImplementedException ();
		}

		public void OnDrawContent ()
		{
			throw new System.NotImplementedException ();
		}

		public void OnHideContent ()
		{
			throw new System.NotImplementedException ();
		}


		public IParentItem Parent { get; set; }


		/*static bool ShowWebServerMenu ()
		{
			List<IChildItem> items = new List<IChildItem> ();
			var portItem = new ItemWithNumericInput("Port", settings.WebServerSettings.Port, 1, ushort.MaxValue);
			portItem.OnValueChanged+= delegate(int value) 
			{
				new Thread(delegate() {
					settings.WebServerSettings.Port = value;
					settings.Save();
				}).Start();
			};
			var startItem = new ItemWithCheckBox("Start server", Webserver.Instance.IsRunning,
				delegate(bool running)
				{ 

					bool isRunning = running;
					if(running){
						var step = new StepContainer(
							delegate() 
							{
								Webserver.Instance.Stop();
								return true;
							},
							"Stopping", "Failed to stop");
						var dialog = new ProgressDialog("Web Server",step);
						dialog.Show();
						isRunning = Webserver.Instance.IsRunning;
					}
					else{
						var step1 = new StepContainer(()=>{Webserver.Instance.Start(portItem.Value); return true;}, "Starting REST", "Failed To Start REST");
						var step2 = new StepContainer(()=>{return Webserver.Instance.LoadPage();}, "Loading Webpage", "Failed to load page");
						var stepDialog = new StepDialog("Web Server", new List<IStep>{step1,step2}, "Webserver started");
						isRunning = stepDialog.Show();
					}
					return isRunning;
				} 
			);

			//items.Add(portItem);
			items.Add(startItem);
			//Show the menu
			Menu m = new Menu ("Web Server", items);
			m.Show ();
			return false;
		}*/

	}
}

