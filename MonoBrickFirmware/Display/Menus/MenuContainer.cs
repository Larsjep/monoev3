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
		protected IChildItem activeMenuItem;
		protected CancellationTokenSource cancelSource;
		protected IChildItem topMenu;
		protected ManualResetEvent resume = new ManualResetEvent(false);
		protected bool continueRunning = true;
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
			cancelSource = new CancellationTokenSource ();
			Lcd.Instance.SaveScreen ();
			while (continueRunning) {
				cancelSource = new CancellationTokenSource ();
				resume.Reset ();
				while (!cancelSource.Token.IsCancellationRequested) 
				{
					activeMenuItem.OnDrawContent ();
					buttonAction = false;
					var action = Buttons.Instance.GetKeypress (cancelSource.Token);
					buttonAction = true;
					switch (action) 
					{
						case Buttons.ButtonStates.Down: 
							activeMenuItem.OnDownPressed ();
							 break;
						case Buttons.ButtonStates.Up:
							activeMenuItem.OnUpPressed ();
							break;
						case Buttons.ButtonStates.Escape:
							activeMenuItem.OnEscPressed ();
							break;
						case Buttons.ButtonStates.Enter:
							activeMenuItem.OnEnterPressed ();
							break;
						case Buttons.ButtonStates.Left:
							activeMenuItem.OnLeftPressed ();
							break;
						case Buttons.ButtonStates.Right:
							activeMenuItem.OnRightPressed ();
							break;
					}

				}
				resume.WaitOne ();
			}
			Lcd.Instance.LoadScreen ();
		}

		public void Stop()
		{
			resume.Set();
			continueRunning = false;
			activeMenuItem.OnHideContent();
			cancelSource.Cancel ();
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


		public void SuspendEvents (IChildItem item)
		{
			continueRunning = true;
			activeMenuItem.OnHideContent();
			cancelSource.Cancel();	
		}

		public void ResumeEvents (IChildItem item)
		{
			resume.Set();
			continueRunning = true;
		}
		#endregion


	}
}

