using System;

namespace MonoBrickWebServer
{
	public class UrlModel
	{
		public UrlModel (string url, string description)
		{
			Url = url;
			Description = description;
		}
		public string Url { get; private set; }
		public string Description { get; private set; }
	}
}

