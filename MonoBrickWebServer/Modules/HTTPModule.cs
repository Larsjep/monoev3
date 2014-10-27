using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace MonoBrickWebServer.Modules
{
  public class HTTPModule: NancyModule
  {

    public HTTPModule()
    {
      Get["/Images/{title}"] = parameter =>
      {
        return Response.AsImage(@"Images/" + (string) parameter.title);
      };

      Get["/"] = _ =>
      {
        return View["index"];
      };

    }
  }
}