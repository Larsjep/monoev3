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


	public class DebugSettings{
		[XmlElement("Port")]
		private int port = 12345;

		[XmlElement("TerminateWithEscape")]
		private bool terminateWithEscape = true;

		public bool TerminateWithEscape	
		{
			get { return terminateWithEscape; }
			set { terminateWithEscape = value; }
		}

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

	public class SoundSettings{
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

	[XmlRoot("ConfigRoot")]
	public class FirmwareSettings
	{
		private static object readWriteLock = new object();
		private static string SettingsFileName = "/mnt/bootpar/firmwareSettings.xml";


		[XmlElement("GeneralSettings")]
		public GeneralSettings GeneralSettings { get; set; }

		[XmlElement("WiFiSettings")]
		public WiFiSettings WiFiSettings { get; set; }

		[XmlElement("DebugSettings")]
		public DebugSettings DebugSettings{ get; set; }

		[XmlElement("SoundSettings")]
		public SoundSettings SoundSettings{ get; set; }

		[XmlElement("WebServerSettings")]
		public WebServerSettings WebServerSettings{ get; set; }


		public FirmwareSettings ()
		{
			GeneralSettings = new GeneralSettings();
			WiFiSettings = new WiFiSettings();
			DebugSettings = new DebugSettings();
			SoundSettings = new SoundSettings();
			WebServerSettings = new WebServerSettings();	
		}

		public bool Save()
		{
			lock (readWriteLock) {
				TextWriter textWriter = null;
				try {
					XmlSerializer serializer = XmlHelper.CreateSerializer(typeof(FirmwareSettings));
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

		public FirmwareSettings Load()
		{
			lock (readWriteLock) {
				TextReader textReader = null;
				try{
					XmlSerializer deserializer = XmlHelper.CreateSerializer(typeof(FirmwareSettings));
					textReader = new StreamReader (SettingsFileName);
					Object obj = deserializer.Deserialize (textReader);
					FirmwareSettings myNewSettings = (FirmwareSettings)obj;
					textReader.Close ();
					return myNewSettings;
				}
				catch (Exception exp) 
				{ 
					Console.WriteLine("Exception during settings load: " + exp.Message);
					Console.WriteLine(exp.StackTrace);
				}
				if(textReader!= null)
					textReader.Close();
			}
			return null;
		}
	}
}

