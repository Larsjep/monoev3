using System;

namespace MonoBrickFirmware.IO
{
	/// <summary>
	/// Sensor mode when using a touch sensor
    /// </summary>
    public enum TouchMode { 
		/// <summary>
		/// On or off mode
		/// </summary>
		Boolean = SensorMode.Mode0, 
		
		/// <summary>
		/// Raw mode
		/// </summary>
		Raw = SensorMode.Mode1
	};
    
	/// <summary>
	/// Class used for touch sensor. Works with both EV3 and NXT
	/// </summary>
	public class TouchSensor : AnalogSensor, ISensor{
		private bool nxtConnected;
		private const int boolCutOff = 2000;
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.EV3.TouchSensor"/> class in boolean mode
		/// </summary>
		public TouchSensor (SensorPort port) : this(port,TouchMode.Boolean)
		{
			
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.TouchSensor"/> class.
		/// </summary>
		/// <param name="port">Port.</param>
		/// <param name="mode">Mode.</param>
		public TouchSensor (SensorPort port, TouchMode mode) : base(port)
		{
			Mode = mode;
		}
		
		
		/// <summary>
		/// Initilize the sensor.
		/// </summary>
		public void Initialize ()
		{
			Read();
		}
		
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public string ReadAsString ()
		{
			string s = "";
			if (Mode == TouchMode.Boolean) {
				if (IsPressed()) {
					s = "On";
				} 
				else {
					s = "Off";
				}
			} 
			else {
				s = Read().ToString();
			}
			return s;
		}
		
		private short ReadRaw(){
			nxtConnected = (GetSensorType() == SensorType.NXTTouch);
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
			short rawValue = ReadRaw();
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
		/// Read the value. In bool mode this will return 1 or 0
		/// </summary>
		public int Read()
		{
			short rawValue = ReadRaw();
			if (Mode == TouchMode.Boolean) {
				Convert.ToInt32(IsPressed());
			}
			return (int) rawValue;
		}
		
		/// <summary>
		/// Gets or sets the mode.
		/// </summary>
		/// <value>The mode.</value>
		public TouchMode Mode {
			get{return (TouchMode) this.mode;}
			set{this.mode = (SensorMode)value;}
		}
	}
}

