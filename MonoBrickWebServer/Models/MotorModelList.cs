using System;
using System.Collections;
using MonoBrickFirmware.Movement;

namespace MonoBrickWebServer.Models
{
	public class MotorModelList : IEnumerable
	{
		private IMotorModel[] motors = new IMotorModel[4];
		public MotorModelList (bool useDummy)
		{
			MotorPort[] portList = { MotorPort.OutA, MotorPort.OutB, MotorPort.OutC, MotorPort.OutD };
			for (int i = 0; i < 4; i++) 
			{
				if (useDummy) 
				{
					motors [i] = new DummyMotorModel (portList [i]);
				} 
				else 
				{
					motors [i] = new MotorModel (portList [i]);
				}
			}
		}

		public IMotorModel this[MotorPort port]
		{
			get
			{
				return motors[(int)port];
			}
		}

		public IMotorModel this[int index]
		{
			get
			{
				return motors[index];
			}
		}

		public IMotorModel this[string indexString]
		{
			get
			{
				return motors[MotorIndexToInt(indexString)];
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (object o in motors)
			{
				if(o == null)
				{
					break;
				}
				yield return o;
			}
		}

		public void Update()
		{
			foreach (var motor in motors) 
			{
				motor.Update ();
			}
		}

		private int MotorIndexToInt (string index)
		{
			int motorIndex = 0;
			if (int.TryParse (index, out motorIndex)) 
			{
				motorIndex = motorIndex -1;//convert to array index
			} 
			else
			{
				char charIndex;
				if (char.TryParse(index, out charIndex))
				{
					motorIndex = (int) charIndex;
					if (motorIndex >= 97 && motorIndex < 101) //a-d
						motorIndex = motorIndex - 97;
					if (motorIndex >= 65 && motorIndex < 69) //A-D
						motorIndex = motorIndex - 65;
				}
				else
				{
					switch (index.ToLower())
					{
					case "outa":
						motorIndex = 0;
						break;
					case "outb":
						motorIndex = 1;
						break;
					case "outc":
						motorIndex = 2;
						break;
					case "outd":
						motorIndex = 3;
						break;
					}
				}
			}
			return motorIndex;
		}
	}
}

