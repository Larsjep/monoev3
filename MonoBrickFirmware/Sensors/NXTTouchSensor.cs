using System;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Class used for touch sensor. Works with both EV3 and NXT
	/// </summary>
	public class NXTTouchSensor : AnalogSensor{
		private const int NXTCutoff = 512;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.Sensors.NXTTouchSensor"/> class.
		/// </summary>
		/// <param name="port">Senor Port.</param>
		public NXTTouchSensor (SensorPort port) : base(port)
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
			return base.ReadPin1As10Bit();//NXT
		}
		
		/// <summary>
		/// Determines whether the touch sensor is pressed.
		/// </summary>
		/// <returns><c>true</c> if the sensor is pressed; otherwise, <c>false</c>.</returns>
		public bool IsPressed ()
		{
			short rawValue = (short)ReadRaw();
			if(rawValue < NXTCutoff)
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
			return "NXT Touch";
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

