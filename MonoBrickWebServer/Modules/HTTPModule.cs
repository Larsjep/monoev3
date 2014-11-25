using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace MonoBrickWebServer.Modules
{
  public class HTTPModule: NancyModule
  {
	private Dictionary<string,Func<dynamic,dynamic>> urlActionDictionary = new Dictionary<string, Func<dynamic,dynamic>>();
    public HTTPModule()
    {
		urlActionDictionary.Add ("/Images/{title}", p => {return Response.AsImage(@"Images/" + (string) p.title);}); 
		urlActionDictionary.Add ("/", p => {return View["index"];});
		foreach (KeyValuePair<string,Func<dynamic,dynamic>> pair in urlActionDictionary) 
		{
			this.Get[pair.Key] = parameter =>
	    	{
				return pair.Value(parameter);
	    	};
	    	Console.WriteLine(pair.Key);
		} 
    }
  }
}