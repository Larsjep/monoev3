using System;
using System.Xml;
using System.Xml.Serialization;
namespace MonoBrickFirmware.Settings
{

	public interface IFirmwareSettings
	{
		GeneralSettings GeneralSettings { get; set; }
		WiFiSettings WiFiSettings { get; set; }
		SoundSettings SoundSettings{ get; set; }
		WebServerSettings WebServerSettings{ get; set; }
		bool Save ();
		void Load();
	}

	public class WebServerSettings
	{
		[XmlElement("Port")]
		private int port = 80;

		public int Port
		{
			get { return port; }
			set { port = value; }
		}
	}

	public class WiFiSettings
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

	public class GeneralSettings
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

}

