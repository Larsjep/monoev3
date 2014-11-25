using System;
using Nancy;
using System.Collections.Generic;

namespace MonoBrickWebServer
{
	public class MonoBrickModule : NancyModule
	{
		private Dictionary<string,Func<dynamic,dynamic>> getDictionary = new Dictionary<string, Func<dynamic,dynamic>>();
    	protected void AddGetRequest(string url, Func<dynamic, dynamic> action)
    	{
			this.Get[url] = parameter =>
	    	{
				return action(parameter);
	    	};	
    	}
		
		public void LoadURL(int port)
		{
				


		}	
	}
}

