using System;
using MonoBrickFirmware.Display;
using System.Collections.Generic;
using System.Threading;
using MonoBrickFirmware.UserInput;
using System.Collections.ObjectModel;

namespace MonoBrickFirmware.Display.Menus
{
	
	public class MenuContainer : IParentItem
	{
		private bool buttonAction = false;
		private ButtonEvents buttonEvents;
		protected IChildItem activeMenuItem;
		protected IChildItem topMenu;
		protected ManualResetEvent stop = new ManualResetEvent(false);
		protected bool continueRunning = true;

		private void ExecuteButtonAction(Action action)
		{
			buttonAction = true;
			action ();
			activeMenuItem.OnDrawContent ();
			buttonAction = false;
		}


		private void CreateButtonEvents ()
		{
			buttonEvents = new ButtonEvents ();
			buttonEvents.DownPressed += () => ExecuteButtonAction (activeMenuItem.OnDownPressed);
			buttonEvents.UpPressed += () => ExecuteButtonAction (activeMenuItem.OnUpPressed);
			buttonEvents.LeftPressed += () => ExecuteButtonAction (activeMenuItem.OnLeftPressed);
			buttonEvents.RightPressed += () => ExecuteButtonAction (activeMenuItem.OnRightPressed);
			buttonEvents.EnterPressed += () => ExecuteButtonAction (activeMenuItem.OnEnterPressed);
			buttonEvents.EscapePressed += () => ExecuteButtonAction (activeMenuItem.OnEscPressed);
		}

		public MenuContainer(Menu startMenu)
		{
			activeMenuItem = startMenu;
			activeMenuItem.Parent = this;
			startMenu.SetAsTopMenu ();
			topMenu = startMenu;
			
		}




		/// <summary>
		/// Show menu. This is a blocking call. Will end when Stop is called 
		/// </summary>
		public void Show()
		{
			stop = new ManualResetEvent (false);
			CreateButtonEvents ();
			activeMenuItem.OnDrawContent ();
			stop.WaitOne ();
		}


		public void Stop()
		{
			buttonEvents.Kill ();
			stop.Set ();
		}

		#region IParentMenuItem implementation

		public void SetFocus (IChildItem item)
		{
			activeMenuItem = item;
			if (!buttonAction) 
			{
				activeMenuItem.OnDrawContent ();
			}
		}

		public void RemoveFocus (IChildItem item)
		{
			
		}

		public void RequestRedraw (IChildItem item)
		{
			activeMenuItem.OnDrawContent ();
		}


		public void SuspendButtonEvents ()
		{
			if (buttonEvents != null)
			{
				buttonEvents.Kill ();
				buttonEvents = null;
				Console.WriteLine ("Killing thread");
			}
			Console.WriteLine ("Suspend");
		}

		public void ResumeButtonEvents ()
		{
			if (buttonEvents == null)
			{
				CreateButtonEvents ();
				activeMenuItem.OnDrawContent ();
				Console.WriteLine ("Creating thread");
			}
			Console.WriteLine ("Resume");
		}
		#endregion
	}
}

