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
			AddGetRequest ("/ev3", GetEV3Info); 
			AddGetRequest ("/ev3/motor", GetAllMotorInfo);
			AddGetRequest ("/ev3/motor/{index}", GetMotorInfo);
			AddGetRequest ("ev3/motor/{index}/setpower/{power}", SetMotorPower); 
			AddGetRequest (@"ev3/motor/{index}/powerprofile/power={power}&rampup={rampup}&constant={constant}&rampdown={rampdown}&brake={brake}", SetMotorPowerProfile ); 
			AddGetRequest ("ev3/motor/{index}/setspeed/{speed}",SetMotorSpeed); 
			AddGetRequest ("ev3/motor/{index}/speedprofile/speed={speed}&rampup={rampup}&constant={constant}&rampdown={rampdown}&brake={brake}", SetMotorSpeedProfile); 
			AddGetRequest ("ev3/motor/{index}/break", SetMotorSpeedBrake); 
			AddGetRequest ("ev3/motor/{index}/off", SetMotorOff); 
			AddGetRequest ("ev3/motor/{index}/resettacho", ResetMotorTacho);

			AddGetRequest ("ev3/sensor", GetAllSensors);
			AddGetRequest ("ev3/sensor/{index}", GetSensor);
			AddGetRequest ("ev3/sensor/{index}/nextmode", GetSensorNextMode);
			AddGetRequest ("ev3/sensor/{index}/previousmode", GetSensorPreviousMode);
			AddGetRequest ("/ev3/lcd/screenshot", GetSecreenShot);
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
			return HttpStatusCode.OK;
		}

		private dynamic SetMotorPowerProfile (dynamic parameters)
		{
			var m = EV3.Motors[(string)parameters.index]; 
			bool brake = ((string)parameters.brake).ToBoolean(); 
			m.PowerProfile((sbyte)parameters.power, (uint)parameters.rampup, (uint)parameters.constant, (uint)parameters.rampdown, brake); 
			return HttpStatusCode.OK;		
		}

		private dynamic SetMotorSpeed (dynamic parameters)
		{
			EV3.Motors[(string)parameters.index].SetSpeed((sbyte)parameters.speed); 
			return HttpStatusCode.OK;		
		}

		private dynamic SetMotorSpeedProfile (dynamic parameters)
		{
			var m = EV3.Motors[(string)parameters.index]; 
			bool brake = ((string)parameters.brake).ToBoolean(); 
			m.SpeedProfile((sbyte)parameters.speed, (uint)parameters.rampup, (uint)parameters.constant, (uint)parameters.rampdown, brake); 
			return HttpStatusCode.OK;		
		}

		private dynamic SetMotorSpeedBrake (dynamic parameters)
		{
			EV3.Motors[(string)parameters.index].Break(); 
			return HttpStatusCode.OK;		
		}

		private dynamic SetMotorOff (dynamic parameters)
		{
			EV3.Motors[(string)parameters.index].Off (); 
			return HttpStatusCode.OK;		
		}

		private dynamic ResetMotorTacho (dynamic parameters)
		{
			EV3.Motors[(string)parameters.index].ResetTacho(); 
			return HttpStatusCode.OK;		
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
			return HttpStatusCode.OK;		
		}

		private dynamic GetSensorPreviousMode(dynamic parameters)
		{
			EV3.Sensors[(string)parameters.index].PreviousMode();
			return HttpStatusCode.OK;		
		}

		private dynamic GetSecreenShot (dynamic parameters)
		{
			string directory = Directory.GetCurrentDirectory();
			EV3.LCD.TakeScreenShot(directory, "screenshot");
			return Response.AsImage(Path.Combine(directory, "screenshot"));
		}
	}
}

