using System;
using System.Xml.Serialization;
using System.IO;

namespace EV3MonoBrickSimulator.Settings
{
	[XmlRoot("ConfigRoot")]
	public class SimulatorSettings
	{
		public string SettingsFileName{ get; set;}
		public SimulatorSettings()
		{
			SettingsFileName = "SimulatorSettings.xml";
			WiFiSettings = new WiFiSettings();
			VersionSettings = new VersionSettings ();
			BootSettings = new BootSettings ();
			ProgramManagerSettings = new ProgramManagerSettings ();
		}

		[XmlElement("WiFiSettings")]
		public WiFiSettings WiFiSettings { get ; set;}

		[XmlElement("VersionSettings")]
		public VersionSettings VersionSettings { get ; set;}

		[XmlElement("BootSettings")]
		public BootSettings BootSettings { get ; set;}

		[XmlElement("ProgramManagerSettings")]
		public ProgramManagerSettings ProgramManagerSettings { get ; set;}


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
				VersionSettings = loadSettings.VersionSettings;
				BootSettings = loadSettings.BootSettings;
				ProgramManagerSettings = loadSettings.ProgramManagerSettings;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
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

	public class BootSettings
	{
		[XmlElement("StartUpDir")]
		private string startUpDir = Directory.GetCurrentDirectory();

		[XmlElement("ExecutionDelay")]
		private int executionDelay = 4000;

		[XmlElement("TurnOffDelay")]
		private int turnOffDelay = 5000;


		public int ExecutionDelay {
			get{return executionDelay; }
			set { executionDelay = value; }
		}

		public int TurnOffDelay {
			get{return turnOffDelay; }
			set { turnOffDelay = value; }
		}

		public string StartUpDir
		{
			get{return startUpDir;}
			set{startUpDir = value;}
		}
	}

	public class ProgramManagerSettings
	{
		[XmlElement("AOTCompileTimeMs")]
		private int aotCompileTimeMs = 1500;

		public int AotCompileTimeMs {
			get{return aotCompileTimeMs; }
			set { aotCompileTimeMs = value; }
		}
	}

}

