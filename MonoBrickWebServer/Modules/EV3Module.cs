using System;
using System.Collections.Generic;
using System.Dynamic;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Extensions;
using MonoBrickWebServer.Models;
using Nancy;

namespace MonoBrickWebServer.Modules
{
	
	public class EV3Module : NancyModule
	{
		public static EV3Model EV3;
    	public EV3Module()
		{
		    Get	["/status"] = parameter =>
		    {
				EV3.Update();
				return Response.AsJson(EV3, HttpStatusCode.OK);
		    };

			Get	["/sensor/{index}"] = parameter =>
			{
				return Response.AsJson(EV3.Sensors[(string)parameter.index], HttpStatusCode.OK);
			};


			Get	["/sensor/{index}/nextmode"] = parameter =>
			{
				ISensorModel s = EV3.Sensors[(string)parameter.index];
				s.NextMode();
				return "Selected next mode on sensor " + s.Port;
			};

			Get	["/sensor/{index}/previousmode"] = parameter =>
			{
				ISensorModel s = EV3.Sensors[(string)parameter.index];
				s.PreviousMode();
				return "Previous next mode on sensor " + s.Port;
			};

			Get	["/motor/{index}"] = parameter =>
			{
				IMotorModel m = EV3.Motors[(string)parameter.index];
				m.Update();
				return Response.AsJson(m, HttpStatusCode.OK);
			};

			Get	["/motor/{index}/setpower/{power}"] = parameter =>
			{
				IMotorModel m = EV3.Motors[(string)parameter.index];
				m.SetPower((sbyte)parameter.power);
				return "Motor " + m.Port + " power set to " + (string) parameter.power;
			};

			Get	["/motor/{index}/powerprofile/power={power}&rampup={rampup}&constant={constant}&rampdown={rampdown}&brake={brake}"] = parameter =>
			{
				IMotorModel m = EV3.Motors[(string)parameter.index];
				bool brake = ((string)parameter.brake).ToBoolean();
				m.SpeedProfile((sbyte)parameter.power, (uint)parameter.rampup, (uint)parameter.constant, (uint)parameter.rampdown, brake);
				return "Motor " + m.Port + " power profile with power " + (string) parameter.power + " and " +  ((uint)parameter.rampup + (uint)parameter.constant + (uint)parameter.rampdown) + " steps. Brake set to " + brake;
			};

			Get["/motor/{index}/setspeed/{speed}"] = parameter =>
			{
				IMotorModel m = EV3.Motors[(string)parameter.index];
				m.SetSpeed((sbyte)parameter.speed);
				return "Motor " + m.Port + " speed set to " +  (string)parameter.speed;
			};

			Get	["/motor/{index}/speedprofile/speed={speed}&rampup={rampup}&constant={constant}&rampdown={rampdown}&brake={brake}"] = parameter =>
			{
				IMotorModel m = EV3.Motors[(string)parameter.index];
				bool brake = ((string)parameter.brake).ToBoolean();
				m.SpeedProfile((sbyte)parameter.speed, (uint)parameter.rampup, (uint)parameter.constant, (uint)parameter.rampdown, brake);
				return "Motor " + m.Port + " power profile with speed " + (string) parameter.speed + " and " +  ((uint)parameter.rampup + (uint)parameter.constant + (uint)parameter.rampdown) + " steps. Brake set to " + brake;
			};

			Get	["/motor/{index}/break/"] = parameter =>
		    {
				IMotorModel m = EV3.Motors[(string)parameter.index];
				m.Break();
				return "Motor " + m.Port + " was set to break";
		    };
		    
		    Get	["/motor/{index}/off/"] = parameter =>
		    {
				IMotorModel m = EV3.Motors[(string)parameter.index];
				m.Off();
				return "Motor " + m.Port + " was set to off ";
		    };

		    Get	["/motor/{index}/resettacho"] = parameter =>
		    {
				IMotorModel m = EV3.Motors[(string)parameter.index];
				m.ResetTacho();
				return "Motor " + m.Port + " reset it's tachometer";
		    };
	  	}
	}
}

