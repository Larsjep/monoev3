using System;
using MonoBrickFirmware.Connections;
using System.Net.NetworkInformation;
using System.Linq;


namespace MonoBrickFirmwareSimulation.Mock
{
	public class WiFiMock : IWiFiDevice
	{
		private bool isOn = false;
		public WiFiMock ()
		{
		}

		public bool TurnOn (string ssid, string password, bool useEncryption, int timeout = 0)
		{
			if (!isOn) 
			{
				System.Threading.Thread.Sleep (15000);
				isOn = true;
				return true;
			}
			return false;
		}

		public void TurnOff ()
		{
			System.Threading.Thread.Sleep (5000);
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

