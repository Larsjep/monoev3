using System;
namespace MonoBrickFirmware.Settings
{

	public interface IFirmwareSettings
	{
		IGeneralSettings GeneralSettings { get; set; }
		IWiFiSettings WiFiSettings { get; set; }
		ISoundSettings SoundSettings{ get; set; }
		IWebServerSettings WebServerSettings{ get; set; }
		bool Save ();
		void Load();
	}

	public class IWebServerSettings
	{
		public int Port{ get; set;}
	}

	public class IWiFiSettings
	{
		public string SSID { get; set;}
		public string Password{ get; set;}
		public bool Encryption{ get; set;}
	}

	public class IGeneralSettings{
		public bool CheckForSwUpdatesAtStartUp	{ get; set;}

		public bool ConnectToWiFiAtStartUp{ get; set;}
	}

	public class ISoundSettings
	{
		public bool EnableSound{ get; set;}
		public int Volume{ get; set;}
	}

}

