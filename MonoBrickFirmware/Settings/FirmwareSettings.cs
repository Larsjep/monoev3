namespace MonoBrickFirmware.Settings
{
	public static class FirmwareSettings
	{
		static FirmwareSettings()
		{
			Instance = new EV3FirmwareSettings();
		}
		internal static IFirmwareSettings Instance{ get; set;}
		public static GeneralSettings GeneralSettings { get{return Instance.GeneralSettings; } set{Instance.GeneralSettings = value;} }
		public static WiFiSettings WiFiSettings { get{return Instance.WiFiSettings; } set{Instance.WiFiSettings = value;} }
		public static SoundSettings SoundSettings{ get{return Instance.SoundSettings; } set{Instance.SoundSettings = value;} }
		public static WebServerSettings WebServerSettings{ get{return Instance.WebServerSettings; } set{Instance.WebServerSettings = value;} }
		public static bool Save (){return Instance.Save();}
		public static void Load(){Instance.Load();}
	}
}

