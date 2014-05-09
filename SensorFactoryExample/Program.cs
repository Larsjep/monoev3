using System;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Sensors;
using System.Threading;

namespace SensorFactoryExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			object sensorLock = new object();
			bool run = true;
			ISensor[] sensor = new ISensor[4];
			for (int i = 0; i < 4; i++) {
				sensor [i] = null;
			}
			SensorListner listner = new SensorListner();
			listner.SensorAttached += delegate(ISensor obj) {
				lock(sensorLock){
					if(obj != null){
						sensor[(int)obj.Port] = obj;
						Console.WriteLine(obj.GetSensorName() + " attached on " + obj.Port);
					}
				}	
			};
			listner.SensorDetached += delegate(SensorPort obj) {
				lock(sensorLock){
					Console.WriteLine(sensor[(int)obj] + " detached from " + obj);
					sensor[(int)obj] = null;
				}	
			};
			ButtonEvents buts = new ButtonEvents ();
			buts.EscapePressed += delegate 
			{ 
				run = false;
			};
			while (run) {
						
				lock (sensorLock) {
					
					/*for (int i = 0; i < sensor.Length; i++) {
						if (sensor[i] != null) {
							typeLabel [i].Text = sensor[i].GetSensorName ();
							modeLabel [i].Text = sensor[i].SelectedMode ();
							valueLabel[i].Text = sensor[i].ReadAsString ();
						} else {
							typeLabel [i].Text = "Not connected";
							modeLabel [i].Text = "-";
							valueLabel [i].Text = "-";
						}
					}*/
				}
				System.Threading.Thread.Sleep(1000);
			}
			listner.Kill();
		}
	}
}
