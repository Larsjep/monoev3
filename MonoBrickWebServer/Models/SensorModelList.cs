using System;
using System.Collections;
using MonoBrickFirmware.Sensors;

namespace MonoBrickWebServer.Models
{
	public class SensorModelList : IEnumerable
	{
		private ISensorModel[] sensors = new ISensorModel[4];
		public SensorModelList (bool useDummy)
		{
			SensorPort[] portList = { SensorPort.In1, SensorPort.In2, SensorPort.In3, SensorPort.In4};
			for (int i = 0; i < 4; i++) 
			{
				sensors [i] = new SensorModel (portList [i], useDummy);
			}
		}

		public ISensorModel this[SensorPort port]
		{
			get
			{
				return sensors[(int) port];
			}
		}

		public ISensorModel this[int index]
		{
			get
			{
				return sensors[index];
			}
		}

		public ISensorModel this[string indexString]
		{
			get
			{
				return sensors[SensorIndexToInt(indexString)];
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (object o in sensors)
			{
				if(o == null)
				{
					break;
				}
				yield return o;
			}
		}

		private int SensorIndexToInt (string index)
		{
			int sensorIndex = 0;
			if (int.TryParse (index, out sensorIndex)) 
			{
				sensorIndex = sensorIndex -1;//convert to array index
			} 
			else
			{
				switch (index.ToLower())
				{
				case "in1":
					sensorIndex = 0;
					break;
				case "in2":
					sensorIndex = 1;
					break;
				case "in3":
					sensorIndex = 2;
					break;
				case "in4":
					sensorIndex = 3;
					break;
				}
			}
			return sensorIndex;
		}
	}
}

