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
		[XmlElement("DebugPort")]
		private int debugPort = 12345;
		
		[XmlElement("TerminateDebugWithEscape")]
		private bool terminateDebugWithEscape = true;
		
		public FirmwareSettings ()
		{
			
		}
		
		public bool TerminateDebugWithEscape	
		{
			get { return terminateDebugWithEscape; }
			set { terminateDebugWithEscape = value; }
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

