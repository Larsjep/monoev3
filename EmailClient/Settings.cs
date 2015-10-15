using System;
using System.Xml.Serialization;
using System.IO;

namespace EmailClient
{
	[XmlRoot("ConfigRoot")]
	public class EmailSettings
	{
		public string SettingsFileName{ get; set;}
		public EmailSettings()
		{
			SettingsFileName = "EmailSettings.xml";
			UserSettings = new UserSettings();
		}

		[XmlElement("UserSettings")]
		public UserSettings UserSettings { get ; set;}

		public bool Save()
		{
			TextWriter textWriter = null;
			try {
				XmlSerializer serializer = new XmlSerializer(typeof(EmailSettings));
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
				XmlSerializer deserializer = new XmlSerializer(typeof(EmailSettings));
				textReader = new StreamReader (SettingsFileName);
				Object obj = deserializer.Deserialize (textReader);
				var loadSettings = (EmailSettings)obj;
				textReader.Close ();
				UserSettings = loadSettings.UserSettings;
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

	public class UserSettings
	{
		
		[XmlElement("Password")]
		private string password = "";

		[XmlElement("Smtp")]
		private string smpt = "smtp.gmail.com";

		[XmlElement("Port")]
		private int port = 587;

		[XmlElement("EnableSsl")]
		private bool enableSsl = true;

		[XmlElement("User")]
		private string user = "";


		public string Password
		{
			get{return password; }
			set { password = value; }
		}

		public string Smtp
		{
			get{return smpt; }
			set { smpt = value; }
		}

		public int Port {
			get{return port; }
			set { port = value; }
		}

		public bool EnableSsl {
			get{return enableSsl; }
			set { enableSsl = value; }
		}

		public string User
		{
			get{return user; }
			set { user = value; }
		}

	}
}

