using MonoBrickFirmware.Connections;
using System.Net.NetworkInformation;
using System.Linq;
using System;

namespace EV3MonoBrickSimulator.Stub
{
	public class WiFiStub : IWiFiDevice
	{
		private bool isOn = false;

		public int TurnOnTimeMs{ get; set;}
		public int TurnOffTimeMs{ get; set;}

		public bool TurnOn (string ssid, string password, bool useEncryption, int timeout = 0)
		{
			if (!isOn) 
			{
				Console.WriteLine (TurnOnTimeMs);
				System.Threading.Thread.Sleep (TurnOnTimeMs);
				isOn = true;
				return true;
			}
			return false;
		}

		public void TurnOff ()
		{
			System.Threading.Thread.Sleep (TurnOffTimeMs);
			isOn = false;
		}

		public bool IsLinkUp ()
		{
			return isOn;
		}

		public string Gateway ()
		{
			if (IsLinkUp ()) {
				var card = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
				if(card == null) return null;
				var address = card.GetIPProperties().GatewayAddresses.FirstOrDefault();
				return address.Address.ToString();
			}
			return null;
		}

		public string GetIpAddress ()
		{
			if (IsLinkUp()) {
				NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces ();
				foreach (var ni in interfaces) {
					foreach (var addr in ni.GetIPProperties().UnicastAddresses) {
						if (addr.Address.ToString () != "127.0.0.1")
							return addr.Address.ToString ();					
					}
				}
			}
			return "Unknown";
		}
	}
}

