
namespace MonoBrickWebServer
{
	using System;
	using System.Web;
	using System.Web.UI;

	public partial class Default : System.Web.UI.Page
	{
		public void button1Clicked (object sender, EventArgs args)
		{
			this.Sensor1ValueText.Text = "Sensor value is 4";
		}
	}
}

