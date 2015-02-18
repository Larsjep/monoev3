using MonoBrickFirmware.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboDog
{
    public class ExternalMotor : IMotor
    {
        private readonly MonoBrickFirmware.Movement.Motor _someExternalClassWithoutInterface;


        public ExternalMotor(MonoBrickFirmware.Movement.Motor someExternalClassWithoutInterface)
        {
            _someExternalClassWithoutInterface = someExternalClassWithoutInterface;


        }


        public sbyte GetSpeed()
        {
            throw new NotImplementedException();
        }

        public int GetTachoCount()
        {
            throw new NotImplementedException();
        }

        public System.Threading.WaitHandle PowerProfile(sbyte power, uint rampUpSteps, uint constantSpeedSteps, uint rampDownSteps, bool brake)
        {
            throw new NotImplementedException();
        }

        public System.Threading.WaitHandle PowerProfileTime(byte power, uint rampUpTimeMs, uint constantSpeedTimeMs, uint rampDownTimeMs, bool brake)
        {
            throw new NotImplementedException();
        }

        public void ResetTacho()
        {
            //throw new NotImplementedException();
        }

        public void SetSpeed(sbyte speed)
        {
            _someExternalClassWithoutInterface.SetSpeed(speed); //throw new NotImplementedException();
        }

        public System.Threading.WaitHandle SpeedProfile(sbyte speed, uint rampUpSteps, uint constantSpeedSteps, uint rampDownSteps, bool brake)
        {
            throw new NotImplementedException();
        }

        public System.Threading.WaitHandle SpeedProfileTime(sbyte speed, uint rampUpTimeMs, uint constantSpeedTimeMs, uint rampDownTimeMs, bool brake)
        {
            throw new NotImplementedException();
        }

        public MonoBrickFirmware.Movement.OutputBitfield BitField
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<MonoBrickFirmware.Movement.MotorPort> PortList
        {
            get { throw new NotImplementedException(); }
        }

        public void Brake()
        {
            throw new NotImplementedException();
        }

        public void CancelPolling()
        {
            throw new NotImplementedException();
        }

        public bool IsRunning()
        {
            throw new NotImplementedException();
        }

        public void Off()
        {
           // throw new NotImplementedException();
        }

        public void SetPower(sbyte power)
        {
            throw new NotImplementedException();
        }

        public System.Threading.WaitHandle WaitForMotorsToStartAndStop()
        {
            throw new NotImplementedException();
        }

        public System.Threading.WaitHandle WaitForMotorsToStop()
        {
            throw new NotImplementedException();
        }
    }
}
