using System;
using System.Xml.Serialization;
using System.IO;

namespace EV3MonoBrickSimulator.Settings
{
	[XmlRoot("ConfigRoot")]
	public class SimulatorSettings
	{
		public string SettingsFileName{ get; private set;}
		public SimulatorSettings()
		{
			SettingsFileName = "SimulatorSettings.xml";
			WiFiSettings = new WiFiSettings();
			VersionSettings = new VersionSettings ();
		}

		[XmlElement("WiFiSettings")]
		public WiFiSettings WiFiSettings { get ; set;}

		[XmlElement("VersionSettings")]
		public VersionSettings VersionSettings { get ; set;}


		public bool Save()
		{
			TextWriter textWriter = null;
			try {
				XmlSerializer serializer = new XmlSerializer(typeof(SimulatorSettings));
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
			return false;
		}

		public bool Load()
		{
			TextReader textReader = null;
			bool ok = true;
			try{
				XmlSerializer deserializer = new XmlSerializer(typeof(SimulatorSettings));
				textReader = new StreamReader (SettingsFileName);
				Object obj = deserializer.Deserialize (textReader);
				var loadSettings = (SimulatorSettings)obj;
				textReader.Close ();
				WiFiSettings = loadSettings.WiFiSettings;
			}
			catch
			{
				ok = false;
			}
			if(textReader!= null)
				textReader.Close();
			return ok;
		}
	}

	public class VersionSettings
	{
		[XmlElement("ImageVersion")]
		private string imageVersion = "1.0.0.0";

		[XmlElement("AddInVersion")]
		private string addInVersion = null;

		[XmlElement("Repository")]
		private string repository = @"http://monobrick.dk/MonoBrickFirmwareRelease/release/";

		public string Repository {
			get{return repository; }
			set { repository = value; }
		}

		public string ImageVersion {
			get{return imageVersion; }
			set { imageVersion = value; }
		}

		public string AddInVersion {
			get{return addInVersion; }
			set { addInVersion = value; }
		}
	}


	public class WiFiSettings
	{
		[XmlElement("TurnOnTimeMs")]
		private int turnOnTimeMs = 3500;

		[XmlElement("TurnOffTimeMs")]
		private int turnOffTimeMs = 2000;

		public int TurnOnTimeMs {
			get{return turnOnTimeMs; }
			set { turnOnTimeMs = value; }
		}

		public int TurnOffTimeMs {
			get{return turnOffTimeMs; }
			set { turnOffTimeMs = value; }
		}
	}


}

