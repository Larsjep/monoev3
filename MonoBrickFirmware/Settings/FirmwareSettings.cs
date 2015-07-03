using System;
using System.Xml.Serialization;
using System.IO;
using MonoBrickFirmware.Tools;

namespace MonoBrickFirmware.Settings
{
	public class WebServerSettings{
		[XmlElement("Port")]
		private int port = 80;

		public int Port
		{
			get { return port; }
			set { port = value; }
		}
	}

	public class WiFiSettings{
		[XmlElement("SSID")]
		private string ssid = "YourSSID";

		[XmlElement("Password")]
		private string password = "YourPassword";

		[XmlElement("Encryption")]
		private bool encryption = true;

		public string SSID {
			get{return ssid; }
			set { ssid = value; }
		}
		public string Password{
			get{return  password; }
			set {  password = value; }
		}

		public bool Encryption	
		{
			get { return encryption; }
			set { encryption = value; }
		}
	}

	public class GeneralSettings{
		[XmlElement("CheckForSwUpdatesAtStartUp")]
		private bool checkForSwUpdatesAtStartUp = false;

		[XmlElement("ConnectToWiFiAtStartUp")]
		private bool connectToWiFiAtStartUp = false;

		public bool CheckForSwUpdatesAtStartUp	
		{
			get { return checkForSwUpdatesAtStartUp; }
			set { checkForSwUpdatesAtStartUp = value; }
		}

		public bool ConnectToWiFiAtStartUp	
		{
			get { return connectToWiFiAtStartUp; }
			set { connectToWiFiAtStartUp = value; }
		}
	}

	public class SoundSettings
	{
		[XmlElement("Volume")]
		private int volume = 60;

		[XmlElement("EnableSound")]
		private bool enableSound = true;

		public bool EnableSound	
		{
			get { return enableSound; }
			set { enableSound = value; }
		}


		public int Volume	
		{
			get { return volume; }
			set { volume = value; }
		}
	}

	public interface IFirmwareSettings
	{
		GeneralSettings GeneralSettings { get; set; }
		WiFiSettings WiFiSettings { get; set; }
		SoundSettings SoundSettings{ get; set; }
		WebServerSettings WebServerSettings{ get; set; }
		bool Save ();
	  void Load();
	}


	public static class FirmwareSettings
	{
		static FirmwareSettings()
		{
			Settings = new EV3FirmwareSettings();
		}
		public static IFirmwareSettings Settings{ get; set;}
		public static GeneralSettings GeneralSettings { get{return Settings.GeneralSettings; } set{Settings.GeneralSettings = value;} }
		public static WiFiSettings WiFiSettings { get{return Settings.WiFiSettings; } set{Settings.WiFiSettings = value;} }
		public static SoundSettings SoundSettings{ get{return Settings.SoundSettings; } set{Settings.SoundSettings = value;} }
		public static WebServerSettings WebServerSettings{ get{return Settings.WebServerSettings; } set{Settings.WebServerSettings = value;} }
		public static bool Save (){return Settings.Save();}
	  public static void Load(){Settings.Load();}
	}

	[XmlRoot("ConfigRoot")]
	public class EV3FirmwareSettings : IFirmwareSettings
	{
		private object readWriteLock = new object();
    //private const string SettingsFileName = "/mnt/bootpar/firmwareSettings.xml";
    private const string SettingsFileName = "firmwareSettings.xml";

    public EV3FirmwareSettings()
		{
			GeneralSettings = new GeneralSettings();
			WiFiSettings = new WiFiSettings();
			SoundSettings = new SoundSettings();
			WebServerSettings = new WebServerSettings();
		}

		[XmlElement("GeneralSettings")]
		public GeneralSettings GeneralSettings { get ; set;}

		[XmlElement("WiFiSettings")]
		public WiFiSettings WiFiSettings { get ; set;}

		[XmlElement("SoundSettings")]
		public SoundSettings SoundSettings { get ; set;}

		[XmlElement("WebServerSettings")]
		public WebServerSettings WebServerSettings { get ; set;}

		public bool Save()
		{
			lock (readWriteLock) {
				TextWriter textWriter = null;
				try {
					XmlSerializer serializer = XmlHelper.CreateSerializer(typeof(EV3FirmwareSettings));
					textWriter = new StreamWriter (SettingsFileName);
					serializer.Serialize (textWriter, this);
					textWriter.Close ();
					return true;
				} 
				catch (Exception exp) 
				{ 
					Console.WriteLine("Exception during settings save: " + exp.Message);
					Console.WriteLine(exp.StackTrace);
				}
				if(textWriter!= null)
					textWriter.Close();
			}
			return false;
		}

		public void Load()
		{
			lock (readWriteLock) 
			{
				TextReader textReader = null;
				try{
					XmlSerializer deserializer = XmlHelper.CreateSerializer(typeof(EV3FirmwareSettings));
					textReader = new StreamReader (SettingsFileName);
					Object obj = deserializer.Deserialize (textReader);
					var loadSettings = (EV3FirmwareSettings)obj;
					textReader.Close ();
          GeneralSettings = loadSettings.GeneralSettings;
          WiFiSettings = loadSettings.WiFiSettings;
          SoundSettings = loadSettings.SoundSettings;
          WebServerSettings = loadSettings.WebServerSettings;
				}
				catch (FileNotFoundException exp)
				{
				  Save();
				}
				if(textReader!= null)
					textReader.Close();
			}
		}
	} 


}

