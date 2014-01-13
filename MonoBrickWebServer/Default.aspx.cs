namespace MonoBrickWebServer
{
	using System;
	using System.Web;
	using System.Web.UI;
	using MonoBrickFirmware.Sensors;

	public partial class Default : System.Web.UI.Page
	{
		private EV3UltrasonicSensor sensor = null;
		
		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			//sensor = new EV3UltrasonicSensor(SensorPort.In1);
		}
		
		public void button1Clicked (object sender, EventArgs args)
		{
			if (sensor == null) 
			{
				sensor = new EV3UltrasonicSensor(SensorPort.In1);
			}
			this.Sensor1ValueText.Text = sensor.ReadAsString();
		}
	}
}

