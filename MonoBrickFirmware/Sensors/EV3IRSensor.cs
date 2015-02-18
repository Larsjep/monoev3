using System;
using MonoBrickFirmware.Extensions;


namespace MonoBrickFirmware.Sensors
{
	/// <summary>
	/// Sensor mode when using a EV3 IR Sensor
    /// </summary>
    public enum IRMode { 
		/// <summary>
		/// Use the IR sensor as a distance sensor
		/// </summary>
		Proximity = UARTMode.Mode0, 
		
		/// <summary>
		/// Use the IR sensor to detect the location of the IR Remote
		/// </summary>
		Seek  = UARTMode.Mode1,
		
		/// <summary>
		/// Use the IR sensor to detect wich Buttons where pressed on the IR Remote
		/// </summary>
		Remote  = UARTMode.Mode2,
		
	};
	
	/// <summary>
	/// IR channels
	/// </summary>
	public enum IRChannel{
		#pragma warning disable 
		One = 0, Two = 1, Three = 2, Four = 3
		#pragma warning restore
	};
	
	/// <summary>
	/// Class for IR beacon location.
	/// </summary>
	public class BeaconLocation{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.Sensors.BeaconLocation"/> class.
		/// </summary>
		/// <param name="location">Location.</param>
		/// <param name="distance">Distance.</param>
		public BeaconLocation(sbyte location, sbyte distance){this.Location = location; this.Distance = distance;}
		
		/// <summary>
		/// Gets the location of the beacon ranging from minus to plus increasing clockwise when pointing towards the beacon
		/// </summary>
		/// <value>The location of the beacon.</value>
		public sbyte Location{get;private set;}
		
		/// <summary>
		/// Gets the distance of the beacon in CM (0-100)
		/// </summary>
		/// <value>The distance to the beacon.</value>
		public sbyte Distance{get; private set;}
	
	}
	
	
	
	
	/// <summary>
	/// Class for the EV3 IR sensor
	/// </summary>
	public class EV3IRSensor : UartSensor{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.Sensors.EV3IRSensor"/> class.
		/// </summary>
		/// <param name="port">Port.</param>
		public EV3IRSensor (SensorPort port) : this(port, IRMode.Proximity)
		{
				
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrickFirmware.Sensors.EV3IRSensor"/> class.
		/// </summary>
		/// <param name="port">Senosr port.</param>
		/// <param name="mode">IR Mode.</param>
		public EV3IRSensor (SensorPort port, IRMode mode) :  base(port)
		{
			base.Initialise(base.uartMode);
			Mode = mode;
			Channel = IRChannel.One;
		}
		
		/// <summary>
		/// Gets or sets the IR mode. 
		/// </summary>
		/// <value>The mode.</value>
		public IRMode Mode {
			get{return (IRMode) base.uartMode;}
			set{SetMode((UARTMode) value);}
		}
		
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>The value as a string</returns>
		public override string ReadAsString ()
		{
			string s = "";
			switch ((IRMode)base.uartMode)
			{
			    case IRMode.Proximity:
			        s = ReadDistance() + " cm";
			        break;
			   case IRMode.Remote:
			        s = ReadRemoteCommand() + " on channel " + Channel;
			        break;
			   case IRMode.Seek:
			        BeaconLocation location = ReadBeaconLocation();
			        s = "Location: " + location.Location + " Distance: " + location.Distance + " cm";
			        break;
			}
			return s;
		}
		
		/// <summary>
		/// Read the sensor value. The returned value depends on the mode. Distance in proximity mode. 
		/// Remote command number in remote mode. Beacon location in seek mode. 
		/// </summary>
		public int Read(){
			int value = 0;
			switch ((IRMode)base.uartMode)
			{
			    case IRMode.Proximity:
			        value = ReadDistance();
			        break;
			   case IRMode.Remote:
			        value = (int)ReadRemoteCommand();
			        break;
			   case IRMode.Seek:
			        value = (int)ReadBeaconLocation().Location;
			        break;
			}
			return value;	
		}
		
		/// <summary>
		/// Read the distance of the sensor in CM (0-100). This will change mode to proximity
		/// </summary>
		public int ReadDistance()
		{
			if (Mode != IRMode.Proximity) {
				Mode = IRMode.Proximity;
			}
			return ReadByte();			
		}
		
		/// <summary>
		/// Reads commands from the IR-Remote. This will change mode to remote
		/// </summary>
		/// <returns>The remote command.</returns>
		public byte ReadRemoteCommand ()
		{
			if (Mode != IRMode.Remote) {
				Mode = IRMode.Remote;
			}
			return ReadBytes (4) [(int)Channel];
		}
		
		/// <summary>
		/// Gets the beacon location. This will change the mode to seek
		/// </summary>
		/// <returns>The beacon location.</returns>
		public BeaconLocation ReadBeaconLocation(){
			if (Mode != IRMode.Seek) {
				Mode = IRMode.Seek;
			}
			byte[] data = ReadBytes(4*2);
			return new BeaconLocation((sbyte)data[(int)Channel*2], (sbyte)data[((int)Channel*2)+1]);  
		}
		
		/// <summary>
		/// Gets or sets the IR channel used for reading remote commands or beacon location
		/// </summary>
		/// <value>The channel.</value>
		public IRChannel Channel{get;set;}
		
		public override string GetSensorName ()
		{
			return "EV3 IR";
		}
		
		public override void SelectNextMode()
		{
			Mode = Mode.Next();
			return;
		}
		
		public override void SelectPreviousMode ()
		{
			Mode = Mode.Previous();
			return;
		}
		
		public override int NumberOfModes ()
		{
			return Enum.GetNames(typeof(IRMode)).Length;
		
		}
        
        public override string SelectedMode ()
		{
			return Mode.ToString();
		}
		
		
	}
}

