using System;
using System.IO;
using System.Xml.Serialization;
using MonoDevelop.Core;

namespace MonoBrickAddin
{
	public class UserSettings
	{
		const string SettingsPropId = "MonoBrickAddin.Settings";

		public string IPAddress;
		public string DebugPort;
		public bool Verbose;
		public string LastUploadHash;
		private static UserSettings _instance;

		public UserSettings()
		{
			IPAddress = "0";
			DebugPort = "12345";
			Verbose = false;
			LastUploadHash = "";
		}

		public static UserSettings Instance
		{
			get{ 
				if (_instance == null) {
					_instance = PropertyService.Get<UserSettings> (SettingsPropId);
					if (_instance == null)
						_instance = new UserSettings ();
				}

				return _instance;
			}
		}

		public static void Save()
		{
			PropertyService.Set(SettingsPropId, _instance);
			PropertyService.SaveProperties();
		}
	}
}

