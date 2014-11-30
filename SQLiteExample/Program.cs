using System;
using SQLite;
using System.IO;
using MonoBrickFirmware.Sensors;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Extensions;
using System.Collections.Generic;
using System.Linq;

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

	public class SensorReading
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
		private const int NumberOfReadings = 10; 
		public static void Main (string[] args)
		{
			db = new SQLiteConnection ("MySensorDb");
			CreateDb ();

			for(int i = 0; i < NumberOfReadings; i++)
			{
				System.Threading.Thread.Sleep (1000);
				Console.WriteLine ("Reading and storing sensor reading (" + (i + 1) + " out of " + NumberOfReadings + ")");
				foreach (KeyValuePair<int,ISensor> keyPair in sensorsDictionary) {
					AddSensorValueToDb(keyPair.Key, keyPair.Value.ReadAsString ());
				}
			}

			Console.WriteLine ("Viewing all sensor values that are less than 5 seconds old");
			DateTime compareTime = DateTime.Now.Subtract (new TimeSpan (0, 0, 0, 0, 5000));//Now minus 5500ms
			var timeQuery = db.Table<SensorReading> ().Where (s => s.Time > compareTime);
			foreach (var val in timeQuery) 
			{
				Console.WriteLine (val.Time.ToLongTimeString ().PadRight (20) + val.Value);
			}

			Console.WriteLine ("Viewing all sensor values on port 2 that are less than 5 seconds old");
			int port2Id = sensorsDictionary.ElementAt(1).Key;
			var portQuery = timeQuery.Where(s => s.SensorId == port2Id);
			foreach (var val in portQuery) 
			{
				Console.WriteLine (val.Time.ToLongTimeString().PadRight (20) + val.Value);
			}

			Console.WriteLine ("Viewing all sensor values on port 1 ever recorded (including other times the program was run)");
			var joinQuery = db.Query<SensorReading> ("select V.* from SensorReading V inner join Sensor S" + " on V.SensorId = S.Id where S.Port =?", "In1");
			foreach (var val in joinQuery) 
			{
				Console.WriteLine (val.Time.ToLongTimeString().PadRight (20) + val.Value);
			}
		}

		public static void CreateDb ()
		{
			Console.WriteLine ("Creating tables in database");
			db.CreateTable<Sensor> ();
			Console.WriteLine ("Creating sensors in database");
			db.CreateTable<SensorReading> ();
			Console.WriteLine ("Storing sensor indexes");
			ISensor[] sensors = SensorFactory.GetSensorArray();
			foreach (var sensor in sensors) 
			{
				var sensorToInsert = new Sensor(){Port = sensor.Port.ToString(), Mode = sensor.SelectedMode(), Name = sensor.GetSensorName() };
				db.Insert(sensorToInsert);//id in the sensorToInsert is updated
				sensorsDictionary.Add(sensorToInsert.Id, sensor);
			}
		}

		public static void AddSensorValueToDb(int id, string sensorValue)
		{
			db.Insert(new SensorReading() 
			{
					SensorId = id,
	   		 		Time = DateTime.Now,
					Value = sensorValue
    		});								
		}
	}
}
