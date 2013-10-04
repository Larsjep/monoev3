using System;
using MonoBrickFirmware.Native;
using System.Threading;

namespace MonoBrickFirmware.IO
{
	public class Buttons : IDisposable
	{
		UnixDevice dev;
		MemoryArea buttonMem;
		public const int ButtonCount = 6;
		EventWaitHandle stopPolling = new ManualResetEvent(false);
		const int pollTime = 50;
		public Buttons ()
		{
			dev = new UnixDevice("/dev/lmd_ui");
			buttonMem = dev.MMap(ButtonCount, 0);
			new Thread(ButtonPollThread).Start();
		}
				
		void ButtonPollThread()
		{
			while (!stopPolling.WaitOne(pollTime))
			{
				byte[] buttonData = new byte[6];
				//buttonMem.
			}
		}

		#region IDisposable implementation
		void IDisposable.Dispose ()
		{
			stopPolling.Set();
			dev.Dispose();
		}
		#endregion
	}
}

