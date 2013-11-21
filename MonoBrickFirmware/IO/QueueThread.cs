using System;
using System.Collections.Generic;
using System.Threading;

namespace MonoBrickFirmware.IO
{
	public class QueueThread : IDisposable
	{
		public QueueThread ()
		{
			Thread t = new Thread(ThreadFunc);
			t.IsBackground = true;
			t.Start();
		}
		private Queue<Action> queue = new Queue<Action>();
		private EventWaitHandle stop = new EventWaitHandle(false, EventResetMode.ManualReset);
		private void ThreadFunc()
		{
			while (!stop.WaitOne(0))
			{
				Action action = null;
				lock (queue)
				{
					if (queue.Count > 0)
					{
						action = queue.Dequeue();						
					}
					else
					{
						Monitor.Wait(queue);					
					}
				}
				if (action != null)
					action();
			}
		}
		
		public void Enqueue(Action a)
		{
			lock (queue)
			{
				queue.Enqueue(a);
				Monitor.Pulse(queue);
			}
		}
					
		public void Dispose()
		{
			lock (queue)
			{
				stop.Set();
				Monitor.Pulse(queue);
			}
		}								
	}
}

