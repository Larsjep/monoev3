using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Specialized;
using System.Xml;

namespace StartupApp
{
	
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
		
		[XmlElement("ConnectAtStartUp")]
		private bool connectAtStartUp = false;
		
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
		
		public bool ConnectAtStartUp	
		{
			get { return connectAtStartUp; }
			set { connectAtStartUp = value; }
		}
		
		public bool Encryption	
		{
			get { return encryption; }
			set { encryption = value; }
		}
	}
	
	public class GeneralSettings{
		[XmlElement("CheckForSwUpdatesAtStartUp")]
		private bool checkForSwUpdatesAtStartUp = true;
		
		public bool CheckForSwUpdatesAtStartUp	
		{
			get { return checkForSwUpdatesAtStartUp; }
			set { checkForSwUpdatesAtStartUp = value; }
		}
	}
	
	[XmlRoot("ConfigRoot")]
	public class FirmwareSettings
	{
		[XmlElement("GeneralSettings")]
		public GeneralSettings GeneralSettings { get; set; }
		
		[XmlElement("WiFiSettings")]
		public WiFiSettings WiFiSettings { get; set; }

		[XmlElement("DebugSettings")]
		public DebugSettings DebugSettings{ get; set; }

		
		public FirmwareSettings ()
		{
			GeneralSettings = new GeneralSettings();
			WiFiSettings = new WiFiSettings();
			DebugSettings = new DebugSettings();	
		}
		
		public bool SaveToXML (String filepath)
		{
			try {
				XmlSerializer serializer = new XmlSerializer (typeof(FirmwareSettings));
				TextWriter textWriter = new StreamWriter (filepath);
				serializer.Serialize (textWriter, this);
				textWriter.Close ();
				return true;
			} 
			catch{}
			return false;
		}
		
		public FirmwareSettings LoadFromXML (String filepath)
		{
			XmlSerializer deserializer = new XmlSerializer (typeof(FirmwareSettings));
			TextReader textReader = new StreamReader (filepath);
			Object obj = deserializer.Deserialize (textReader);
			FirmwareSettings myNewSettings = (FirmwareSettings)obj;
			textReader.Close ();
			return myNewSettings;
		}
	}
}

