using System;
using SQLite;
using System.IO;
using MonoBrickFirmware.Sensors;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Extensions;
using System.Collections.Generic;

namespace SQLiteExample
{
	public class Sensor
	{
		[PrimaryKey, AutoIncrement]
	    public int Id { get; set; }
	    [MaxLength(5)]
	    public String Port{get; set;}
		public string Mode { get; set; }
		public string Name { get; set; }

	}

	public class SensorValue
	{
	    [PrimaryKey, AutoIncrement]
	    public int Id { get; set; }
	    [Indexed]
	    public int SensorId { get; set; }
	    public DateTime Time { get; set; }
	    public string Value { get; set; }
	}

	class MainClass
	{
		private static Dictionary<int, ISensor>  sensorsDictionary = new Dictionary<int, ISensor>();		
		private static SQLiteConnection db;
		public static void Main (string[] args)
		{
			db = new SQLiteConnection ("MySensorDb");
			CreateDb ();
			for (int i = 0; i < 1; i++) {
				Console.WriteLine ("Reading sensor values " + (i + 1) + " out of 10");
				foreach (KeyValuePair<int,ISensor> keyPair in sensorsDictionary) {
					AddSensorValue (keyPair.Key, keyPair.Value.ReadAsString ());
				}
				System.Threading.Thread.Sleep (1000);
			}
			var q = db.Query<SensorValue> ("select V.* from SensorValue V inner join Sensor S" + " on V.SensorId = S.Id where S.Port =?", "In1");
			foreach (var v in q) 
			{
				Console.WriteLine(v.Value);
			}
			Console.ReadLine();

		}

		public static void CreateDb ()
		{
			Console.WriteLine ("Creating tables in database");
			db.CreateTable<Sensor> ();
			db.CreateTable<SensorValue> ();
			Console.WriteLine ("Creating sensors in database");
			ISensor[] sensors = new ISensor[]{new DummySensor(SensorPort.In1), new DummySensor(SensorPort.In2), new DummySensor(SensorPort.In3), new DummySensor(SensorPort.In4)};
			foreach (var sensor in sensors) 
			{
				var sensorToInsert = new Sensor(){Port = sensor.Port.ToString(), Mode = sensor.SelectedMode(), Name = sensor.GetSensorName() };
				db.Insert(sensorToInsert);//id in the sensorToInsert is updated
				sensorsDictionary.Add(sensorToInsert.Id, sensor);
			}
		}

		public static void AddSensorValue(int id, string sensorValue)
		{
			db.Insert(new SensorValue() 
			{
					SensorId = id,
	   		 		Time = DateTime.Now,
					Value = sensorValue
    		});								
		}
	}

	public class DummySensor : ISensor
	{
		private SensorPort port;
		private enum DummyMode { Raw = 1, Digital = 2 };
		private DummyMode mode = DummyMode.Raw;
		Random rnd = new Random();
		public DummySensor(SensorPort port)
		{
			this.port = port;
		}

		/// <summary>
		/// Reads the sensor value as a string.
		/// </summary>
		/// <returns>
		/// The value as a string
		/// </returns>
		public string ReadAsString()
		{
			if (mode == DummyMode.Digital)
				return rnd.Next(1).ToString();
			return rnd.Next(1024) + " A/D value";
		}

		/// <summary>
		/// Gets the name of the sensor.
		/// </summary>
		/// <returns>The sensor name.</returns>
		public string GetSensorName()
		{
			return "Dummy Sensor";
		}

		/// <summary>
		/// Selects the next mode.
		/// </summary>
		public void SelectNextMode()
		{
			mode = mode.Next();
		}

		/// <summary>
		/// Selects the previous mode.
		/// </summary>
		public void SelectPreviousMode()
		{
			mode = mode.Previous();
		}

		/// <summary>
		/// Numbers the of modes.
		/// </summary>
		/// <returns>The number of modes</returns>
		public int NumberOfModes()
		{
			return Enum.GetNames(typeof(DummyMode)).Length;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>The mode.</returns>
		public string SelectedMode()
		{
			return mode.ToString();
		}

		public SensorPort Port { get { return port; } }
	}

}
