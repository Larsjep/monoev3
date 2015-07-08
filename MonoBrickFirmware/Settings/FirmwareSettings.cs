namespace MonoBrickFirmware.Settings
{
	public static class FirmwareSettings
	{
		static FirmwareSettings()
		{
			Settings = new EV3FirmwareSettings();
		}
		public static IFirmwareSettings Settings{ get; set;}
		public static IGeneralSettings GeneralSettings { get{return Settings.GeneralSettings; } set{Settings.GeneralSettings = value;} }
		public static IWiFiSettings WiFiSettings { get{return Settings.WiFiSettings; } set{Settings.WiFiSettings = value;} }
		public static ISoundSettings SoundSettings{ get{return Settings.SoundSettings; } set{Settings.SoundSettings = value;} }
		public static IWebServerSettings WebServerSettings{ get{return Settings.WebServerSettings; } set{Settings.WebServerSettings = value;} }
		public static bool Save (){return Settings.Save();}
	  	public static void Load(){Settings.Load();}
	}
}

