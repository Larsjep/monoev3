using System;
using MonoBrickFirmware.Settings;
using System.Xml.Serialization;
using System.IO;

namespace MonoBrickFirmwareSimulation.Mock
{
	public class SettingsMock : EV3FirmwareSettings
	{
		public SettingsMock ()
		{
			this.SettingsFileName = "FirmwareSettings.xml";
		}

		public override void Load ()
		{
			lock (readWriteLock) 
			{
				TextReader textReader = null;
				try{
					XmlSerializer deserializer = new XmlSerializer(typeof(SettingsMock));
					textReader = new StreamReader (SettingsFileName);
					Object obj = deserializer.Deserialize (textReader);
					var loadSettings = (SettingsMock)obj;
					textReader.Close ();
					GeneralSettings = loadSettings.GeneralSettings;
					WiFiSettings = loadSettings.WiFiSettings;
					SoundSettings = loadSettings.SoundSettings;
					WebServerSettings = loadSettings.WebServerSettings;
				}
				catch (FileNotFoundException)
				{
					Save();
				}
				if(textReader!= null)
					textReader.Close();
			}

		}

		public override bool Save ()
		{
			lock (readWriteLock) {
				TextWriter textWriter = null;
				try {
					XmlSerializer serializer = new XmlSerializer(typeof(SettingsMock));
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
	}
}

