using System;

namespace MonoBrickFirmware
{
	
	public class MovingAverage
	{
		private UInt32 size;
		private float sum;
		private int index;
		float[] val;
		
		public MovingAverage (UInt32 windowSize, float initialValue = 0.0f)
		{
			   size = windowSize;
			   val = new float[size];
			   SetValue(initialValue);

		}
		
		public float GetAverage()
		{
			return sum/size;	
			
		}
		
		public void Update (float newValue)
		{
			sum = sum -val[index] + newValue;
			val[index] = newValue;
			index++;
			if(index >= size){
			  index = 0;
			}
		}
		
		public float UpdateAndGetAverage (float newValue)
		{
			Update(newValue);
			return GetAverage();
		}
		
		private void SetValue (float value)
		{
			for(int i = 0; i < size; i++)
			{
			     val[i] = value;
			}
			sum = value * size;
			index = 0;
		}
	}
}


