using MonoBrickFirmware.Connections;
using System.Net.NetworkInformation;
using System.Linq;
using System;
using System.Net;
using System.Net.Sockets;

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
				IPHostEntry host;
				string localIP = "";
				host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						localIP = ip.ToString();
						break;
					}
				}
				return localIP;
			}
			return "Unknown";
		}
	}
}

