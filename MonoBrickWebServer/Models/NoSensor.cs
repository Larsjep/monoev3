using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonoBrickFirmware.Sensors;

namespace MonoBrickWebServer.Models
{
  public class NoSensor : ISensor
  {
    public NoSensor(SensorPort port)
    {
      Port = port;
    }

    public string ReadAsString()
    {
      return "0";
    }

    public string GetSensorName()
    {
      return "Not connected";
    }

    public void SelectNextMode()
    {
    
    }

    public void SelectPreviousMode()
    {
    
    }

    public int NumberOfModes()
    {
      return 0;
    }

    public string SelectedMode()
    {
      return "None";
    }

    public SensorPort Port { get; private set; }
  }
}