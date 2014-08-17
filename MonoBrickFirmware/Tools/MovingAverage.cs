using System;

namespace MonoBrickFirmware
{
	
	public class MovingAverage
	{
		private UInt32 size;
		private float sum;
		private int index;
		private float[] val;
		private bool windowIsFull;
		
		public MovingAverage (UInt32 windowSize)
		{
			sum = 0.0f;
			size = windowSize;
			index = 0;
			val = new float[size];
			windowIsFull = false;
		}
		
		public MovingAverage (UInt32 windowSize, float fillValue)
		{
			size = windowSize;
			val = new float[size];
			FillWindow(fillValue);
		}
		
		
		
		public void FillWindow(float value)
		{
			for(int i = 0; i < size; i++)
			{
			     val[i] = value;
			}
			sum = value * size;
			index = 0;
			windowIsFull = true;
		}
		
		public float GetAverage ()
		{
			float average;
			if (windowIsFull) 
			{
				//Console.WriteLine("Get average sum: " + sum + " size: " + size);
				average = sum / size;
			} 
			else 
			{
				if (index == 0) 
				{
					average = 0.0f;
				} 
				else 
				{
					//Console.WriteLine("Get average sum: " + sum + " index: " + index);
					average = sum / index;
				}
			}
			return average;	
		}
		
		public void Update (float newValue)
		{
			sum = sum - val [index] + newValue;
			val [index] = newValue;
			index++;
			if (index >= size) {
				index = 0;
				if (!windowIsFull) 
				{
					windowIsFull = true;
				}
			}
		}
		
		public float UpdateAndGetAverage (float newValue)
		{
			Update(newValue);
			return GetAverage();
		}
		
		
	}
}


