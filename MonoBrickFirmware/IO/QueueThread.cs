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
				lock (queue)
				{
					while (queue.Count > 0)
					{
						Action action = queue.Dequeue();
						action();
					}
					Monitor.Wait(queue);					
				}
			}
		}
		
		public void Enqueue(Action a)
		{
			lock (queue)
			{
				queue.Enqueue(a);
			}
		}
					
		public void Dispose()
		{
			lock (queue)
			{
				stop.Set();
			}
		}								
	}
}

