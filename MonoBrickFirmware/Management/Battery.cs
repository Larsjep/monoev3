using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoBrickFirmware.Sensors;

namespace MonoBrickFirmware.Management
{
    /// <summary>
    /// Battery information
    /// </summary>
    public static class Battery
    {
        private const int batteryCurrentOffset = 28;
        private const int batteryVoltageOffset = 30;

        private const float adcRef = 5.0f; // 5.0 Volts
        private const int adcRes = 4095;

        private const float shuntIn = 0.11f;
        private const float ampCin = 22.0f;
        private const float vce = 0.05f;
        private const float ampVin = 0.5f;

        /// <summary>
        /// Convert from ADC reading to actual units.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static float Convert(int val)
        {
            return (val*adcRef)/adcRes;
        }

        public static float Current
        {
            get { return (Convert(CurrentRaw)/ampCin)/shuntIn; }
        }

        public static float Voltage
        {
            get
            {
                float cinV = Convert(CurrentRaw)/ampCin;
                return Convert(VoltageRaw)/ampVin + cinV + vce;
            }
        }

        private static short CurrentRaw
        {
            get
            {
                return BitConverter.ToInt16(SensorManager.Instance.AnalogMemory.Read(batteryCurrentOffset, 2), 0);
            }
        }

        private static short VoltageRaw
        {
            get
            {
                return BitConverter.ToInt16(SensorManager.Instance.AnalogMemory.Read(batteryVoltageOffset, 2), 0);
            }
        }
    }
}
