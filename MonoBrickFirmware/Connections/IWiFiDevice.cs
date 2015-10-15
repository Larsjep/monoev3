namespace MonoBrickFirmware.Connections
{
	public interface IWiFiDevice
	{
		bool TurnOn (string ssid, string password, bool useEncryption, int timeout = 0);
		void TurnOff ();
		bool IsLinkUp ();
		string Gateway ();
		string GetIpAddress();
	}
}

