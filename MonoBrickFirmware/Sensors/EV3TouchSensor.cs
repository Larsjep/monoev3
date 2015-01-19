using System;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Class used for touch sensor. Works with both EV3 and NXT
	/// </summary>
	public class EV3TouchSensor : AnalogSensor{
		private const int EV3Cutoff = ADCResolution/2;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.Sensors.EV3TouchSensor"/> class.
		/// </summary>
		/// <param name="port">Sensor Port.</param>
		public EV3TouchSensor (SensorPort port) : base(port)
		{
			
		}
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public override string ReadAsString ()
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
			return (int)ReadPin6();//EV3
		}
		
		/// <summary>
		/// Determines whether the touch sensor is pressed.
		/// </summary>
		/// <returns><c>true</c> if the sensor is pressed; otherwise, <c>false</c>.</returns>
		public bool IsPressed ()
		{
			short rawValue = (short)ReadRaw();
			if(rawValue > EV3Cutoff)
				return true;
			return false;
		}
		
		/// <summary>
		/// Read the value. Will return 1 or 0
		/// </summary>
		public int Read()
		{
			return Convert.ToInt32(IsPressed());
		}
		
		public override string GetSensorName ()
		{
			return "EV3 Touch";
		}
		
		public override void SelectNextMode()
		{
			return;
		}
		
		public override void SelectPreviousMode ()
		{
			return;
		}
		
		public override int NumberOfModes ()
		{
			return 1;
		
		}
        
        public override string SelectedMode ()
		{
			return "Analog";
		}
	}
	
	
	
	
	
}

