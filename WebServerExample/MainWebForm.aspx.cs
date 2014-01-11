using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using MonoBrickFirmware.Sensors;
namespace WebServerExample
{
  public partial class MainWebForm : System.Web.UI.Page
  {
    //private static EV3UltrasonicSensor sensor = new EV3UltrasonicSensor(SensorPort.In1);
    protected void Page_Load(object sender, EventArgs e)
    {
    
    }

    protected void SensorTimer_Tick(object sender, EventArgs e)
    {
      //Sensor1ValueTextBox.Text = "Count: " + sensor.ReadAsString();
    }




  }
}