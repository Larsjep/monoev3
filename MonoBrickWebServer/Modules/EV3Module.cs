using System;
using System.Collections.Generic;
using System.Dynamic;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Extensions;
using MonoBrickWebServer.Models;
using Nancy;
using System.IO;

namespace MonoBrickWebServer.Modules
{
	public class EV3Module : MonoBrickModule
	{
		public static EV3Model EV3;
		public EV3Module ()
		{
			
			urlActionDictionary.Add ("/ev3", p => {}); 
			urlActionDictionary.Add ("/ev3/motor", p => {}); 
			urlActionDictionary.Add ("ev3/motor/{index}", p => {}); 
			urlActionDictionary.Add ("ev3/motor/{index}/setpower/{power}", p => {}); 
			urlActionDictionary.Add ("ev3/motor/{index}/powerprofile/power={power}&rampup={rampup}&constant={constant}&rampdown={rampdown}&brake={brake}", p => {}); 
			urlActionDictionary.Add ("ev3/motor/{index}/setspeed/{speed}", p => {}); 
			urlActionDictionary.Add ("ev3/motor/{index}/speedprofile/speed={speed}&rampup={rampup}&constant={constant}&rampdown={rampdown}&brake={brake}", p => {var m = EV3.Motors[(string)p.index]; bool brake = ((string)p.brake).ToBoolean(); m.SpeedProfile((sbyte)p.speed, (uint)p.rampup, (uint)p.constant, (uint)p.rampdown, brake); return "Motor " + m.Port + " power profile with speed " + (string) p.speed + " and " +  ((uint)p.rampup + (uint)p.constant + (uint)p.rampdown) + " steps. Brake set to " + brake;}); 	
			urlActionDictionary.Add ("ev3/motor/{index}/break/", p => {var m = EV3.Motors[(string)p.index]; m.Break(); return "Motor " + m.Port + " was set to break";}); 
			urlActionDictionary.Add ("ev3/motor/{index}/resettacho/", p => {var m = EV3.Motors[(string)p.index]; m.ResetTacho(); return "Motor " + m.Port + " was reset it's tachometer";}); 

		    foreach (KeyValuePair<string,Func<dynamic,dynamic>> pair in urlActionDictionary) 
			{
				this.Get[pair.Key] = parameter =>
		    	{
					return pair.Value(parameter);
		    	};
		    	Console.WriteLine(pair.Key);
			}

			Get	["/ev3"] = parameter =>
		    {
				EV3.Update();
				return Response.AsJson(EV3, HttpStatusCode.OK);
		    };

			/*Get	["/ev3/lcd/screenshot"] = parameter =>
		    {
				string directory = Directory.GetCurrentDirectory();
				EV3.LCD.TakeScreenShot(directory, "screenshot");
				return Response.AsImage(Path.Combine(directory, "screenshot"));
		    };


			Get	["ev3/sensor"] = parameter =>
			{
				EV3.Sensors.Update();
				return Response.AsJson(EV3.Sensors, HttpStatusCode.OK);
			};

			Get	["ev3/sensor/{index}"] = parameter =>
			{
				ISensorModel s = EV3.Sensors[(string)parameter.index];
				s.Update();
				return Response.AsJson(s, HttpStatusCode.OK);
			};


			Get	["ev3/sensor/{index}/nextmode"] = parameter =>
			{
				ISensorModel s = EV3.Sensors[(string)parameter.index];
				s.NextMode();
				return "Selected next mode on sensor " + s.Port;
			};

			Get	["ev3/sensor/{index}/previousmode"] = parameter =>
			{
				ISensorModel s = EV3.Sensors[(string)parameter.index];
				s.PreviousMode();
				return "Previous next mode on sensor " + s.Port;
			};*/

			/*Get ["ev3/motor"] = parameter =>
			{
				EV3.Motors.Update();
				return Response.AsJson(EV3.Motors, HttpStatusCode.OK);
			};*/
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
			var m = EV3.Motors[(string)parameters.index]; 
			m.SetPower((sbyte)parameters.power); 
			return "Motor " + m.Port + " power set to " + (string) parameters.power;
		}

		private dynamic SetMotorPowerProfile (dynamic parameters)
		{
			var m = EV3.Motors[(string)parameters.index]; 
			bool brake = ((string)parameters.brake).ToBoolean(); 
			m.PowerProfile((sbyte)parameters.power, (uint)parameters.rampup, (uint)parameters.constant, (uint)parameters.rampdown, brake); 
			return "Motor " + m.Port + " power profile with power " + (string) parameters.power + " and " +  ((uint)parameters.rampup + (uint)parameters.constant + (uint)parameters.rampdown) + " steps. Brake set to " + brake;
		}

		private dynamic SetMotorSpeed (dynamic parameters)
		{
			var m = EV3.Motors[(string)parameters.index]; 
			m.SetSpeed((sbyte)parameters.speed); 
			return "Motor " + m.Port + " speed set to " +  (string)parameters.speed;
		}


	}
}

