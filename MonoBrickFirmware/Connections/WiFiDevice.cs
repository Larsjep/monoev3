namespace MonoBrickFirmware.Connections
{
	
	public static class WiFiDevice
	{
		static WiFiDevice()
		{
			try
			{
				Device = new EV3WiFiDevice();	
			}	
			catch
			{
				Device = null;
			}
		}
		public static IWiFiDevice Device { get; set;}
		public static bool TurnOn (string ssid, string password, bool useEncryption, int timeout = 0){return Device.TurnOn (ssid, password, useEncryption, timeout);}
		public static void TurnOff (){Device.TurnOff ();}
		public static bool IsLinkUp (){return Device.IsLinkUp ();}
		public static string Gateway (){return Device.Gateway ();}
		public static string GetIpAddress(){return Device.GetIpAddress ();}
	}
}

