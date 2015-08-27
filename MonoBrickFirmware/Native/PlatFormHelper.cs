using System;
using System.IO;

namespace MonoBrickFirmware.Tools
{
	public static class PlatFormHelper
	{
	
		/// <summary>
		/// Platform enumeration
		/// </summary>
		public enum Platform
		{
			/// <summary>
			/// Windows.
			/// </summary>
			Windows,

			/// <summary>
			/// Linux.
			/// </summary>
			Linux,

			/// <summary>
			/// Mac OS.
			/// </summary>
			Mac,

			/// <summary>
			/// EV3.
			/// </summary>
			EV3,

		}
		/// <summary>
		/// Get the operating system
		/// </summary>
		/// <returns>The platform.</returns>
		public static Platform RunningPlatform { 
			get {
				switch (Environment.OSVersion.Platform) {
				case PlatformID.Unix:
					if (Directory.Exists ("/Applications")
					    && Directory.Exists ("/System")
					    && Directory.Exists ("/Users")
					    && Directory.Exists ("/Volumes"))
						return Platform.Mac;
					else if (File.Exists (@"/home/root/lejos/bin/startup")
					         && File.Exists (@"/usr/local/bin/version.txt")
					         && File.Exists (@"/usr/local/bin/repository.txt"))
						return Platform.EV3;
					else
						return Platform.Linux;
				case PlatformID.MacOSX:
					return Platform.Mac;
				default:
					return Platform.Windows;
				}
			}
		}
	}
}

