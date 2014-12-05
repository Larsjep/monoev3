using System;
using System.Collections.Generic;
using System.Dynamic;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Extensions;
using MonoBrickWebServer.Models;
using Nancy;
using System.IO;
using System.Text.RegularExpressions;

namespace MonoBrickWebServer.Modules
{
	public class EV3Module : NancyModule
	{
		internal static EV3Model EV3;
		internal static List<UrlModel> UrlList = new List<UrlModel>();
		private static bool isInitialized = false;
		public EV3Module ()
		{
			//HTTP requests
			AddGetRequest ("/", p => View ["index"], "Websever frontpage");
			AddGetRequest ("/Images/{title}", p => Response.AsImage (@"Images/" + (string)p.title)); 
			AddGetRequest ("/documentation",  p => View ["documentation"]);

			AddGetRequest ("/ev3", GetEV3Info, "Get sensor and motor info"); 
			AddGetRequest ("/ev3/motor", GetAllMotorInfo, "Get motor info");
			AddGetRequest ("/ev3/motor/{index}", GetMotorInfo, "Get specific motor info ", "OutA");
			AddGetRequest ("/ev3/motor/{index}/setpower/{power}", SetMotorPower, "Set motor power", "OutA", "50"); 
			AddGetRequest (@"/ev3/motor/{index}/powerprofile/power={power}&rampup={rampup}&constant={constant}&rampdown={rampdown}&brake={brake}", SetMotorPowerProfile, "Start a motor power profile", "OutA", "50", "100", "300", "100", "true"); 
			AddGetRequest ("/ev3/motor/{index}/setspeed/{speed}", SetMotorSpeed, "Set motor speed", "OutA", "50"); 
			AddGetRequest ("/ev3/motor/{index}/speedprofile/speed={speed}&rampup={rampup}&constant={constant}&rampdown={rampdown}&brake={brake}", SetMotorSpeedProfile, "Start a motor speed profile", "OutA", "80", "150", "400", "150", "true");
			AddGetRequest ("/ev3/motor/{index}/break", SetMotorSpeedBrake, "Break motor", "OutA"); 
			AddGetRequest ("/ev3/motor/{index}/off", SetMotorOff, "Turn motor off", "OutA"); 
			AddGetRequest ("/ev3/motor/{index}/resettacho", ResetMotorTacho, "Reset motor tacho ", "OutA");

			AddGetRequest ("/ev3/sensor", GetAllSensors, "Get all sensor info");
			AddGetRequest ("/ev3/sensor/{index}", GetSensor, "Get specific sensor info", "In1");
			AddGetRequest ("/ev3/sensor/{index}/nextmode", GetSensorNextMode, "Select next sensor mode", "In1");
			AddGetRequest ("/ev3/sensor/{index}/previousmode", GetSensorPreviousMode, "Select previous sensor mode", "In1");
			AddGetRequest ("/ev3/lcd/screenshot", GetSecreenShot, "Get LCD screenshot");

			AddGetRequest ("/urls", p => Response.AsJson (UrlList, HttpStatusCode.OK), "Get a list of URLS");

			isInitialized = true;
	  	}


		private void AddGetRequest (string url, Func<dynamic, dynamic> action)
		{
			AddGetRequest(url, action, null);
		}


		private void AddGetRequest (string url, Func<dynamic, dynamic> action, string description, params string[] args)
		{
			Get [url] = parameter => {
				return action (parameter);
			};
			if (!isInitialized && description != null) {
				if (args == null) 
				{
					UrlList.Add (new UrlModel (url, description)); 	
				} 
				else 
				{
					UrlList.Add (new UrlModel (ReplaceURLWithArgs (url, args), description)); 	
				}
			}
    	}

		private dynamic GetEV3Info (dynamic parameters)
		{
			EV3.Update (); 
			return Response.AsJson (EV3, HttpStatusCode.OK);
		}

	  	private dynamic GetAllMotorInfo (dynamic parameters)
		{
			EV3.Update (); 
			return Response.AsJson (EV3.Motors, HttpStatusCode.OK);
		}

		private dynamic GetMotorInfo (dynamic parameters)
		{
			var m = EV3.Motors[(string)parameters.index];
			m.Update(); 
			return Response.AsJson(m, HttpStatusCode.OK);
		}

		private dynamic SetMotorPower (dynamic parameters)
		{
			EV3.Motors[(string)parameters.index].SetPower((sbyte)parameters.power); 
			return "";
		}

		private dynamic SetMotorPowerProfile (dynamic parameters)
		{
			var m = EV3.Motors[(string)parameters.index]; 
			bool brake = ((string)parameters.brake).ToBoolean(); 
			m.PowerProfile((sbyte)parameters.power, (uint)parameters.rampup, (uint)parameters.constant, (uint)parameters.rampdown, brake); 
			return "";		
		}

		private dynamic SetMotorSpeed (dynamic parameters)
		{
			EV3.Motors[(string)parameters.index].SetSpeed((sbyte)parameters.speed); 
			return "";		
		}

		private dynamic SetMotorSpeedProfile (dynamic parameters)
		{
			var m = EV3.Motors[(string)parameters.index]; 
			bool brake = ((string)parameters.brake).ToBoolean(); 
			m.SpeedProfile((sbyte)parameters.speed, (uint)parameters.rampup, (uint)parameters.constant, (uint)parameters.rampdown, brake); 
			return "";		
		}

		private dynamic SetMotorSpeedBrake (dynamic parameters)
		{
			EV3.Motors[(string)parameters.index].Break(); 
			return "";		
		}

		private dynamic SetMotorOff (dynamic parameters)
		{
			EV3.Motors[(string)parameters.index].Off (); 
			return "";		
		}

		private dynamic ResetMotorTacho (dynamic parameters)
		{
			EV3.Motors[(string)parameters.index].ResetTacho(); 
			return "";		
		}

		private dynamic GetAllSensors(dynamic parameters)
		{
			EV3.Sensors.Update();
			return Response.AsJson(EV3.Sensors, HttpStatusCode.OK);
		}

		private dynamic GetSensor(dynamic parameters)
		{
			ISensorModel s = EV3.Sensors[(string)parameters.index];
			s.Update();
			return Response.AsJson(s, HttpStatusCode.OK);
		}

		private dynamic GetSensorNextMode (dynamic parameters)
		{
			EV3.Sensors[(string)parameters.index].NextMode();
			return "";
		}

		private dynamic GetSensorPreviousMode(dynamic parameters)
		{
			EV3.Sensors[(string)parameters.index].PreviousMode();
			return "";
		}

		private dynamic GetSecreenShot (dynamic parameters)
		{
			string directory = Directory.GetCurrentDirectory();
			EV3.LCD.TakeScreenShot(directory, "screenshot.bmp");
			return Response.AsImage(Path.Combine(directory, "screenshot.bmp"));
		}

		private IEnumerable<string> GetSubStrings(string input, string start, string end)
		{
		    Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
		    MatchCollection matches = r.Matches(input);
		    foreach (Match match in matches)
		        yield return match.Groups[1].Value;
		}

		private string ReplaceURLWithArgs (string url, params string[] args)
		{
			var subStrings = GetSubStrings(url, "{", "}");
			int i = 0;
			foreach(var s in subStrings)
			{
				url = url.Replace("{" + s + "}", args[i]);
				i++;
			}
			return url;
		}

	}
}

