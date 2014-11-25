using System;
using Nancy;
using System.Collections.Generic;
using System.Net;

namespace MonoBrickWebServer
{
	public class MonoBrickModule : NancyModule
	{

		//private Dictionary<string, string> getDictionary = new Dictionary<string, Func<dynamic,dynamic>>();
		protected void AddGetRequest(string url, Func<dynamic, dynamic> action, string loadURL = null)
    	{
			//getDictionary.Add (url, loadURL);
			this.Get[url] = parameter =>
	    	{
				return action(parameter);
	    	};	
    	}

		/*protected void LoadAllURL()
		{
			foreach (KeyValuePair<string,string> key in getDictionary) 
			{
				if (key.Value != null) 
				{
					using(WebClient client = new WebClient()) 
					{
						string s = client.DownloadString(key.Value);
					}
				}			
			}
		}*/
	}
}

