
using System;
using MonoBrickFirmware;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Movement;
using System.Threading;
using MonoBrickFirmware.Sensors;
using MonoBrickFirmware.UserInput;

namespace MindsensorsAngleExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//The MSSensorMUXBase allows you to use the Mindsensors Multiplexer almost like every other sensor
			//The easiest way to use sensors is the following:
			MSSensorMUXBase GyroSensor = new MSSensorMUXBase (SensorPort.In1, MSSensorMUXPort.C1);

			//But that is a general way to use them that doesn't allow to use the TouchSensor and doesn't specify
			//the number of modes of the Sensor. It's better to use the following way:
			MSSensorMUXBase IRSensor = new MSSensorMUXBase (SensorPort.In1, MSSensorMUXPort.C3, IRMode.Remote);

			//You can see that you can use the Modes of the different sensors the same way like when you use them
			//without Multiplexer.

			//The EV3 Touchsensor needs to be traited a bit different:
			MSSensorMUXBase TouchSensor = new MSSensorMUXBase (SensorPort.In1, MSSensorMUXPort.C2, MSSensorMUXMode.TouchMode);

			while (true) {
				System.Threading.Thread.Sleep (500);

				//You can either read one, two, four or eight bytes at once from the sensor.
				//Normally one byte is enough but there are times when you want to read more.
				//Refer to http://mindsensors.com/index.php?module=documents&JAS_DocumentManager_op=downloadFile&JAS_File_id=1394
				//(Supported Sensors) for the suggested length for the different modes
				LcdConsole.WriteLine (Convert.ToString (GyroSensor.Read()) + ", " +
					Convert.ToString (TouchSensor.ReadOneByte()) + ", " + IRSensor.ReadAsString ());
			}
		}
	}
}
