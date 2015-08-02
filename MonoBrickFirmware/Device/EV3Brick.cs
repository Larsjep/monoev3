using System;
using MonoBrickFirmware.Sensors;
using MonoBrickFirmware.Native;
using System.Threading;

namespace MonoBrickFirmware.Device
{
	internal class EV3Brick : IBrick
	{
		private const int batteryCurrentOffset = 28;
		private const int batteryVoltageOffset = 30;

		private const float adcRef = 5.0f; // 5.0 Volts
		private const int adcRes = 4095;

		private const float shuntIn = 0.11f;
		private const float ampCin = 22.0f;
		private const float vce = 0.05f;
		private const float ampVin = 0.5f;


		public float BatteryCurrent()
		{
			return (Convert(CurrentRaw())/ampCin)/shuntIn;
		}

		public float BatteryVoltage()
		{
			float cinV = Convert(CurrentRaw())/ampCin;
			return Convert(VoltageRaw())/ampVin + cinV + vce;
		}

		public void TurnOff ()
		{
			ProcessHelper.RunAndWaitForProcess ("/sbin/shutdown", "-h now");
			Thread.Sleep (120000);
		}

		/// <summary>
		/// Convert from ADC reading to actual units.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		private float Convert(int val)
		{
			return (val*adcRef)/adcRes;
		}
	
		private short CurrentRaw()
		{
			return BitConverter.ToInt16(SensorManager.Instance.AnalogMemory.Read(batteryCurrentOffset, 2), 0);
		}

		private short VoltageRaw()
		{
			return BitConverter.ToInt16(SensorManager.Instance.AnalogMemory.Read(batteryVoltageOffset, 2), 0);
		}

	}
}

