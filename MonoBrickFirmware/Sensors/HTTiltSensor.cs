using System;

namespace MonoBrickFirmware.Sensors
{
	internal enum TiltRegister : byte
    {
        Version = 0x00, ProductId = 0x08, SensorType = 0x10, MeasurementUnits = 0x14,
        XHigh = 0x42, YHigh = 0x43, ZHigh = 0x44, XLow = 0x45, YLow = 0x46,
        ZLow = 0x47 
    };
    
    
    /// <summary>
	/// X Y Z position
	/// </summary>
    public class Position {
        private int x;
        private int y;
        private int z;
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoBrick.NXT.Position"/> class.
        /// </summary>
        /// <param name='x'>
        /// The x coordinate.
        /// </param>
        /// <param name='y'>
        /// The y coordinate.
        /// </param>
        /// <param name='z'>
        /// The z coordinate.
        /// </param>
		public Position(int x, int y, int z) { this.x = x; this.y = y; this.z = z; }
        
		/// <summary>
		/// Gets the x coordinate
		/// </summary>
		/// <value>
		/// The x coordinate
		/// </value>
		public int X { get { return x; } }
        
		/// <summary>
		/// Gets the y coordinate
		/// </summary>
		/// <value>
		/// The y coordinate
		/// </value>
		public int Y { get { return y; } }
        
		/// <summary>
		/// Gets the z coordinate
		/// </summary>
		/// <value>
		/// The z coordinate
		/// </value>
		public int Z { get { return z; } }
    }
	
	
	/// <summary>
	/// HiTechnic tilt compass sensor
	/// </summary>
    public class HiTecTiltSensor : I2CSensorBase, ISensor
    {
        private const byte CompassAddress = 0x02;
        
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBrick.NXT.HiTecCompass"/> class.
		/// </summary>
		public HiTecTiltSensor(SensorPort port) : base(port, CompassAddress, I2CMode.LowSpeed9V) { 
			base.Initialise();
		}

		/// <summary>
		/// Reads the X Y Z position
		/// </summary>
		/// <returns>
		/// The X Y Z position
		/// </returns>
        public Position ReadPosition() {
            byte[] data = ReadRegister((byte)TiltRegister.XHigh,6);
            int x,y,z;
            x = (int) data[0];
            y = (int) data[1];
            z = (int) data[2];
            
            if( x > 127) 
                x -= 256;
            x = x *4 + data[3];
            
            if( y > 127) 
                y -= 256;
            y = y * 4 + data[4];
            
            if( z > 172) 
                z -= 256;
            z = z * 4 +data[5];
            return new Position(x, y, z);
        }
        
		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
        public string ReadAsString()
        {
            Position pos = ReadPosition();
            return "x:" + pos.X + " y:" + pos.Y + " z:" + pos.Z;
        }
    }
}

