using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace MonoBrickWebServer.Modules
{
  public class HTTPModule: MonoBrickModule
  {
    public HTTPModule()
    {
		AddGetRequest ("/Images/{title}", p => Response.AsImage (@"Images/" + (string)p.title)); 
		AddGetRequest ("/", p => View["index"]); 
    }
  }
}