using System;

namespace MonoBrickWebServer
{
	public class DokumentationModel
	{
		public DokumentationModel (string url, string description)
		{
			URL = url;
			Description = description;
		}
		public string URL { get; private set; }
		public string Description { get; private set; }
	}
}

