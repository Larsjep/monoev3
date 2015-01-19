using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MonoBrickFirmware.Sensors;

namespace MonoBrickWebServer.Models
{
  public interface ISensorModel
  {
    void NextMode();
    void PreviousMode();
    void Update();
	void AttachSensor (ISensor sensor);
	void DetachSensor ();
	string Mode { get; }
    string Port { get; }
    string Name { get; }
    string Value { get; }
  }
}
