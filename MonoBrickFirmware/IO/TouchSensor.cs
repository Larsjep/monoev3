using System;

namespace MonoBrickFirmware.IO
{
	/// <summary>
	/// Class used for touch sensor. Works with both EV3 and NXT
	/// </summary>
	public class TouchSensor : AnalogSensor, ISensor{
		private bool nxtConnected;
		private const int boolCutOff = ADCResolution/2;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.EV3.TouchSensor"/> class in boolean mode
		/// </summary>
		public TouchSensor (SensorPort port) : base(port)
		{
			
		}
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public string ReadAsString ()
		{
			string s = "";
			if (IsPressed()) {
				s = "On";
			} 
			else {
				s = "Off";
			}
			return s;
		}
		
		/// <summary>
		/// Reads the raw sensor value
		/// </summary>
		/// <returns>The raw.</returns>
		public int ReadRaw(){
			nxtConnected = ( SensorManager.Instance.GetSensorType(this.port) == SensorType.NXTTouch);
			if(nxtConnected)
				return ReadPin1();//NXT
			return ReadPin6();//EV3
		}
		
		/// <summary>
		/// Determines whether the touch sensor is pressed.
		/// </summary>
		/// <returns><c>true</c> if the sensor is pressed; otherwise, <c>false</c>.</returns>
		public bool IsPressed ()
		{
			short rawValue = (short)ReadRaw();
			if (nxtConnected) {
				if(rawValue < boolCutOff)
					return true;
				return false;	
			} 
			else {
				if(rawValue > boolCutOff)
					return true;
				return false;
			} 
		}
		
		
		/// <summary>
		/// Read the value. Will return 1 or 0
		/// </summary>
		public int Read()
		{
			return Convert.ToInt32(IsPressed());
		}
	}
}

