using System;

namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Sensor mode when using a Sonar sensor
	/// </summary>
    public enum UltraSonicMode { 
		/// <summary>
		/// Result will be in centimeter
		/// </summary>
		Centimeter = 1,
		/// <summary>
		/// Result will be in centi-inch
		/// </summary>
		CentiInch = 2 
	};
    
	internal enum UltraSonicCommand { Off = 00, SingleShot = 0x01, Continuous = 0x02, EventCapture = 0x03, RequestWarmReset = 0x04 };
    internal enum UltraSonicRegister : byte
    {
        Version = 0x00, ProductId = 0x08, SensorType = 0x10, FactoryZeroValue = 0x11, FactoryScaleFactor = 0x12,
        FactoryScaleDivisor = 0x13, MeasurementUnits = 0x14,
        Interval = 0x40, Command = 0x41, Result1 = 0x42, Result2 = 0x43, Result3 = 0x44, Result4 = 0x45, Result5 = 0x46,
        Result6 = 0x47, Result7 = 0x48, Result8 = 0x49, ZeroValue = 0x50, ScaleFactor = 0x51, ScaleDivisor = 0x52,
    };

    internal class UltraSonicSettings{
        private byte zero;
        private byte scaleFactor;
        private byte scaleDivision;
        public UltraSonicSettings(byte zero, byte scaleFactor, byte scaleDivision){
            this.zero = zero;
            this.scaleFactor = scaleFactor;
            this.scaleDivision = scaleDivision;
        }
        public UltraSonicSettings(byte[] data){
            if(data.Length == 3){
                zero = data[0];
                scaleFactor = data[1];
                scaleDivision = data[2];                
            }
            else{
                zero = 0;
                scaleFactor = 0;
                scaleDivision = 0;
            }
        }
        public byte Zero{get{return zero;}}
        public byte ScaleFactor{get{return scaleFactor;}}
        public byte ScaleDivision{get{return scaleFactor;}}
        public override string ToString(){
             return "Zero: " + zero.ToString() + " Scale factor: " + scaleFactor.ToString() + " Scale division: " + scaleDivision.ToString();
        }
    }
        
    /// <summary>
    /// Sonar sensor
    /// </summary>
	public class UltraSonicSensor : I2CSensorBase, ISensor {
        private const byte UltraSonicAddress = 0x02;
		private UltraSonicMode sonarMode;
        
		/// <summary>
		/// Gets or sets the sonar mode.
		/// </summary>
		/// <value>
		/// The sonar mode 
		/// </value>
		public  UltraSonicMode Mode{ get{return sonarMode;} set{sonarMode = value;}}
        
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.NXT.Sonar"/> class in centimeter mode
		/// </summary>
		public UltraSonicSensor(SensorPort port) : this(port,UltraSonicMode.Centimeter) { 
			
		}
        
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.NXT.Sonar"/> class.
		/// </summary>
		/// <param name='mode'>
		/// The sonar mode
		/// </param>
		public UltraSonicSensor(SensorPort port, UltraSonicMode mode) : base(port,UltraSonicAddress,I2CMode.LowSpeed9V)
		{ 
			Mode = mode;
			base.Initialise(); 
		}
        
		/// <summary>
		/// Read the distance in either centiinches or centimeter
		/// </summary>
        public int ReadDistance() {
            int reading = ReadRegister((byte)UltraSonicRegister.Result1, 1)[0];
            if (Mode == UltraSonicMode.CentiInch)
                return (reading * 39370) / 1000;
            return reading;
        }
        
		/// <summary>
		/// Fire a single shot 
		/// </summary>
		public void SingleShot() {
            SetMode(UltraSonicCommand.SingleShot);
        }

		/// <summary>
		/// Turn off the sonar to save power
		/// </summary>
		public void Off() { 
            SetMode(UltraSonicCommand.Off);
        }

		/// <summary>
		/// Do Continuous measurements
		/// </summary>
		public void Continuous() {
            SetMode(UltraSonicCommand.Continuous);
        }

		/// <summary>
		/// Determines whether sonar is off.
		/// </summary>
		/// <returns>
		/// <c>true</c> if sonar is off; otherwise, <c>false</c>.
		/// </returns>
        bool IsOff() {
            if (GetMode() == UltraSonicCommand.Off)
                return true;
            return false;
        }

		/// <summary>
		/// Reset the sensor
		/// </summary>
		public new void Reset() {
            SetMode(UltraSonicCommand.RequestWarmReset);
        }

        private byte GetContinuousInterval() {
            return ReadRegister((byte)UltraSonicRegister.Interval, 1)[0]; 
        }
        
		private void SetContinuousInterval(byte interval)
        {
            WriteRegister((byte)UltraSonicRegister.Interval, interval);
            System.Threading.Thread.Sleep(100);
        }

        private UltraSonicSettings GetFactorySettings() {
            return new UltraSonicSettings(   ReadRegister((byte)UltraSonicRegister.FactoryZeroValue, 1)[0], 
                                        ReadRegister((byte)UltraSonicRegister.FactoryScaleFactor, 1)[0], 
                                        ReadRegister((byte)UltraSonicRegister.FactoryScaleDivisor, 1)[0]);            
        }

        private UltraSonicSettings GetActualSettings() {
            return new UltraSonicSettings(   ReadRegister((byte)UltraSonicRegister.ZeroValue, 1)[0],
                                        ReadRegister((byte)UltraSonicRegister.ScaleFactor, 1)[0],
                                        ReadRegister((byte)UltraSonicRegister.ScaleDivisor, 1)[0]);
        }

        private UltraSonicCommand GetMode() {
            return (UltraSonicCommand)ReadRegister((byte)UltraSonicRegister.Command, 1)[0];
        }
		
		private void SetMode(UltraSonicCommand command) {
            WriteRegister((byte)UltraSonicRegister.Command, (byte)command);
            System.Threading.Thread.Sleep(100);
        }

        /// <summary>
        /// Reads the sensor value as a string.
        /// </summary>
        /// <returns>
        /// The value as a string
        /// </returns>
		public string ReadAsString()
        {
            string s = ReadDistance().ToString();
            if (Mode == UltraSonicMode.CentiInch)
                s = s + " centi-inches";
            else
                s = s + " centimeters";
            return s;
        }
        
    }
}

