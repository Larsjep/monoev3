using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Specialized;
using System.Xml;

namespace StartupApp
{
	[XmlRoot("ConfigRoot")]
	public class FirmwareSettings
	{
		[XmlElement("DebugSetting")]
		private bool debugMode = false;
		
		[XmlElement("DebugPort")]
		private int debugPort = 12345;
		
		public FirmwareSettings ()
		{
			
		}
		
		public bool DebugMode	
		{
			get { return debugMode; }
			set { debugMode = value; }
		}
		
		public int DebugPort
		{
			get { return debugPort; }
			set { debugPort = value; }
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

