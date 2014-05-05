using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;

namespace MonoBrickAddin
{
	public class EV3Options : OptionsPanel
	{
		EV3OptionsPanel mWidget;

		public override Gtk.Widget CreatePanelWidget()
		{
			return mWidget = new EV3OptionsPanel();
		}

		public override void ApplyChanges()
		{
			mWidget.Store();
		}
	}

	[System.ComponentModel.ToolboxItem (true)]
	public partial class EV3OptionsPanel : Gtk.Bin
	{
		public EV3OptionsPanel ()
		{
			this.Build ();

			wEV3IPAddress.Text = UserSettings.Instance.IPAddress;
			wEV3DebugPort.Text = UserSettings.Instance.DebugPort;
			wEV3Verbose.Active = UserSettings.Instance.Verbose;

		}

		public bool Store()
		{
			UserSettings.Instance.IPAddress = wEV3IPAddress.Text;
			UserSettings.Instance.DebugPort = wEV3DebugPort.Text;
			UserSettings.Instance.Verbose = wEV3Verbose.Active;
			UserSettings.Save();

			return true;
		}
	}
}

