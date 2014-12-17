using System;
using System.Threading;
using MonoBrickFirmware.Tools;
namespace MonoBrickFirmware.Sensors
{
	public class SensorListner : IDisposable
	{
		private SensorDetector detector = new SensorDetector();
    	private ManualResetEvent terminate = new ManualResetEvent(false);
	  	private bool run = false;
    	Thread thread = null;
		private int interval = 0;
	  	public event Action<ISensor> SensorAttached = delegate {};
		public event Action<SensorPort> SensorDetached = delegate {};
		
		public SensorListner (): this(1000)
		{
		}
		
		public SensorListner (int interval)
		{
			detector.SensorAttached += this.SensorAttached;
			detector.SensorDetached += this.SensorDetached;
			this.interval = interval;
			thread = new Thread(ListenThread);
      		terminate.Reset();
  			run = true;
      		thread.Start();
		}

		private void ListenThread ()
		{
			while (run) 
			{
			    detector.Update();
				terminate.WaitOne(interval);
			}
		}
		
		public void Dispose ()
 		{
			run = false;
			terminate.Set();
			thread.Join();
    	}
	}
}

