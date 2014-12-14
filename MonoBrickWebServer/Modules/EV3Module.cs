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
		internal static ProgramModelList Programs;
		internal static List<UrlModel> UrlList = new List<UrlModel>();
		internal static ILcdModel LCD;

		private static bool isInitialized = false;
		private static string firstProgramName;

		public EV3Module ()
		{
			if (!isInitialized) 
			{
				firstProgramName = Programs[0].Name;
			}

			//HTTP requests
			AddGetRequest ("/", p => View ["index"], "Webpage for motor and sensor");
			AddGetRequest ("/lcd", p => View ["lcd"], "Webpage for LCD control");
			AddGetRequest ("/files", p => View ["files"], "Webpage for Program control");

			AddGetRequest ("/documentation",  p => View ["documentation"], "This documentation");
			AddGetRequest ("/Images/{title}", p => Response.AsImage (@"Images/" + (string)p.title));//Not added to URL list 


			AddGetRequest ("/ev3", GetEV3Info, "Get sensor and motor info as JSON"); 
			AddGetRequest ("/motor", GetAllMotorInfo, "Get all motor info as JSON");
			AddGetRequest ("/motor/{index}", GetMotorInfo, "Get specific motor info as JSON", "OutA");
			AddGetRequest ("/motor/{index}/setpower/{power}", SetMotorPower, "Set motor power", "OutA", "50"); 
			AddGetRequest ("/motor/{index}/powerprofile/power={power}&rampup={rampup}&constant={constant}&rampdown={rampdown}&brake={brake}", SetMotorPowerProfile, "Start a motor power profile", "OutA", "50", "100", "300", "100", "true"); 
			AddGetRequest ("/motor/{index}/setspeed/{speed}", SetMotorSpeed, "Set motor speed", "OutA", "50"); 
			AddGetRequest ("/motor/{index}/speedprofile/speed={speed}&rampup={rampup}&constant={constant}&rampdown={rampdown}&brake={brake}", SetMotorSpeedProfile, "Start a motor speed profile", "OutA", "80", "150", "400", "150", "true");
			AddGetRequest ("/motor/{index}/break", SetMotorSpeedBrake, "Break motor", "OutA"); 
			AddGetRequest ("/motor/{index}/off", SetMotorOff, "Turn motor off", "OutA"); 
			AddGetRequest ("/motor/{index}/resettacho", ResetMotorTacho, "Reset motor tacho ", "OutA");

			AddGetRequest ("/sensor", GetAllSensors, "Get all sensor info as JSON");
			AddGetRequest ("/sensor/{index}", GetSensor, "Get specific sensor info as JSON", "In1");
			AddGetRequest ("/sensor/{index}/nextmode", GetSensorNextMode, "Select next sensor mode", "In1");
			AddGetRequest ("/sensor/{index}/previousmode", GetSensorPreviousMode, "Select previous sensor mode", "In1");

			AddGetRequest ("/lcd/circle/x={x}&y={y}&radius={radius}&color={color}&fill={fill}", DrawCircle, "Draw a circle on the LCD", "89", "64", "40","true", "true");
			AddGetRequest ("/lcd/clear", ClearLcd, "Clear the LCD", "5", "23");
			AddGetRequest ("/lcd/clearlines/y={y}&count={count}", ClearLine , "Clear lines on the LCD", "5", "23");
			AddGetRequest ("/lcd/ellipse/x={x}&y={y}&radius1={radius1}&radius2={radius2}&color={color}&fill={fill}", DrawEllipse, "Draw an ellipse on the LCD", "89", "64", "40", "20", "true", "true");
			AddGetRequest ("/lcd/hline/x={x}&y={y}&length={length}&color={color}", DrawHLine, "Draw a H line in the LCD", "35", "50", "8","true");
			AddGetRequest ("/lcd/line/xstart={xstart}&ystart={ystart}&xend={xend}&yend={yend}&color={color}", DrawLine, "Draw a line on the LCD", "0", "0", "60", "60", "true");
			AddGetRequest ("/lcd/rectangle/xstart={xstart}&ystart={ystart}&xend={xend}&yend={yend}&color={color}&fill={fill}", DrawRectangle, "Draw a rectable on the LCD", "0", "0", "80", "80", "true", "true");
			AddGetRequest ("/lcd/setpixel/x={x}&y={y}&color={color}", SetPixel , "Set a pixel on the LCD", "10", "34", "true");
			AddGetRequest ("/lcd/screenshot", GetSecreenShot, "Get LCD screenshot as BMP image");
			AddGetRequest ("/lcd/text/x={x}&y={y}&text={text}&color={color}", WriteText, "Write text on the LCD", "20", "10", "Text","true");
			AddGetRequest ("/lcd/textbox/xstart={xstart}&ystart={ystart}&xend={xend}&yend={yend}&text={text}&color={color}&align={align}", WriteTextBox, "Write text in a box on the LCD", "0", "0", "100", "100", "Text","true", "center");
			AddGetRequest ("/lcd/vline/x={x}&y={y}&height={height}&color={color}", DrawVLine, "Draw a V line in the LCD", "30", "20", "4","true");

			AddGetRequest ("/program/", GetAllPrograms, "Get all program info as JSON");
			AddGetRequest ("/program/{name}", this.GetProgram, "Get specific program info as JSON", firstProgramName);
			AddGetRequest ("/program/{name}/start", StartProgram, "Start program", firstProgramName);
			AddGetRequest ("/program/{name}/stop", StopProgram, "Stop program", firstProgramName);
			AddGetRequest ("/program/{name}/delete", DeleteProgram, "Delete program", firstProgramName);

			AddGetRequest ("/urls", p => Response.AsJson (UrlList, HttpStatusCode.OK), "Get a list of urls as JSON");

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
			LCD.TakeScreenShot(directory, "screenshot.bmp");
			return Response.AsImage(Path.Combine(directory, "screenshot.bmp"));
		}

		private dynamic SetPixel (dynamic parameters)
		{
			LCD.SetPixel((int) parameters.x, (int) parameters.y, ((string)parameters.color).ToBoolean());
			return ""; 
		}

		private dynamic ClearLine (dynamic parameters)
		{
			LCD.ClearLines((int) parameters.y, (int) parameters.count);
			return "";
		}

		private dynamic ClearLcd (dynamic parameters)
		{
			LCD.Clear();
			return ""; 
		}

		private dynamic DrawVLine (dynamic parameters)
		{
			LCD.DrawVLine((int)parameters.x,(int)parameters.y, (int)parameters.height, ((string)parameters.color).ToBoolean());
			return ""; 
		}

		private dynamic DrawHLine (dynamic parameters)
		{
			LCD.DrawHLine((int)parameters.x,(int)parameters.y, (int)parameters.length, ((string)parameters.color).ToBoolean());
			return ""; 
		}

		private dynamic WriteText (dynamic parameters)
		{
			LCD.WriteText((int)parameters.x,(int)parameters.y, parameters.text, ((string)parameters.color).ToBoolean());
			return ""; 
		}

		private dynamic WriteTextBox (dynamic parameters)
		{
			LCD.WriteTextBox((int)parameters.xstart,(int)parameters.ystart, (int) parameters.xend, (int) parameters.yend, (string) parameters.text, ((string)parameters.color).ToBoolean(), (string)parameters.align);
			return ""; 
		}

		private dynamic DrawCircle (dynamic parameters)
		{
			LCD.DrawCircle((int)parameters.x,(int)parameters.y, (ushort) parameters.radius, ((string)parameters.color).ToBoolean(),((string)parameters.fill).ToBoolean() ); 
			return "";
		}

		private dynamic DrawEllipse (dynamic parameters)
		{
			LCD.DrawEllipse((int)parameters.x,(int)parameters.y, (ushort) parameters.radius1, (ushort) parameters.radius2, ((string)parameters.color).ToBoolean(), ((string)parameters.fill).ToBoolean() );
			return ""; 
		}

		private dynamic DrawLine (dynamic parameters)
		{
			LCD.DrawLine((int)parameters.xstart,(int)parameters.ystart, (int) parameters.xend, (int) parameters.yend, ((string)parameters.color).ToBoolean());
			return ""; 
		}

		private dynamic DrawRectangle(dynamic parameters)
		{
			LCD.DrawRectangle((int)parameters.xstart,(int)parameters.ystart, (int) parameters.xend, (int) parameters.yend, ((string)parameters.color).ToBoolean(), ((string)parameters.fill).ToBoolean());
			return ""; 
		}

		private dynamic GetAllPrograms(dynamic parameters)
		{
			Programs.Update();
			return Response.AsJson(Programs, HttpStatusCode.OK);
		}

		private dynamic GetProgram(dynamic parameters)
		{
			Programs.Update();
			return Response.AsJson(Programs[(string)parameters.name], HttpStatusCode.OK);
		}

		private dynamic StartProgram(dynamic parameters)
		{
			Programs[(string)parameters.name].Start();
			return "";
		}

		private dynamic StopProgram(dynamic parameters)
		{
			Programs[(string)parameters.name].Stop();
			return "";
		}

		private dynamic DeleteProgram(dynamic parameters)
		{
			Programs[(string)parameters.name].Delete();
			Programs.Update();
			firstProgramName = Programs[0].Name;
			return "";
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

