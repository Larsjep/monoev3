using System;
using MonoBrickFirmware.Display;
using System.Collections.Generic;
using System.Threading;
using MonoBrickFirmware.UserInput;
using System.Collections.ObjectModel;
using MonoBrickFirmware.Tools;
namespace MonoBrickFirmware.Display.Menus
{
	
	public class MenuContainer : IParentItem
	{
		private bool buttonAction = false;
		private ButtonEvents buttonEvents;
		protected IChildItem activeMenuItem;
		protected Menu topMenu;
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
			topMenu = startMenu;
			
		}

		/// <summary>
		/// Show menu. This is a blocking call. 
		/// </summary>
		/// <param name="allowTerminationWithEsc">If set to <c>true</c> esc can be used to terminate menu.</param>
		public void Show(bool allowTerminationWithEsc = false)
		{
			topMenu.SetAsTopMenu (allowTerminationWithEsc);
			stop = new ManualResetEvent (false);
			CreateButtonEvents ();
			activeMenuItem.OnDrawContent ();
			stop.WaitOne ();
		}

		/// <summary>
		/// Stop terminate menu
		/// </summary>
		public void Terminate()
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
			if (item.Equals (topMenu)) 
			{
				Terminate ();
			}	
		}

		public void SuspendButtonEvents ()
		{
			if (buttonEvents != null)
			{
				buttonEvents.Kill ();
				buttonEvents = null;
			}
		}

		public void ResumeButtonEvents ()
		{
			if (buttonEvents == null)
			{
				CreateButtonEvents ();
				activeMenuItem.OnDrawContent ();
			}
		}
		#endregion
	}
}

