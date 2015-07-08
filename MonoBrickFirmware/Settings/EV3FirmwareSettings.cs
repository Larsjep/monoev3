using System;
using System;
using System.Xml.Serialization;
using System.IO;
using MonoBrickFirmware.Tools;

namespace MonoBrickFirmware.Settings
{

	[XmlRoot("ConfigRoot")]
	public class EV3FirmwareSettings : IFirmwareSettings
	{
		private object readWriteLock = new object();
		protected string SettingsFileName = "/mnt/bootpar/firmwareSettings.xml";
		public EV3FirmwareSettings()
		{
			GeneralSettings = new GeneralSettings();
			WiFiSettings = new WiFiSettings();
			SoundSettings = new SoundSettings();
			WebServerSettings = new WebServerSettings();
		}

		[XmlElement("GeneralSettings")]
		public IGeneralSettings GeneralSettings { get ; set;}

		[XmlElement("WiFiSettings")]
		public IWiFiSettings WiFiSettings { get ; set;}

		[XmlElement("SoundSettings")]
		public ISoundSettings SoundSettings { get ; set;}

		[XmlElement("WebServerSettings")]
		public IWebServerSettings WebServerSettings { get ; set;}

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

	public class WebServerSettings : IWebServerSettings
	{
		[XmlElement("Port")]
		private int port = 80;

		public int Port
		{
			get { return port; }
			set { port = value; }
		}
	}

	public class WiFiSettings : IWiFiSettings
	{
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

	public class GeneralSettings : IGeneralSettings
	{
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

	public class SoundSettings: ISoundSettings
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

}

