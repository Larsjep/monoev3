using System;
using MonoBrickFirmware.Native;

namespace EV3MonoBrickSimulator
{
	public class SystemCallsStub : ISystemCalls
	{
		public event Action OnShutDown;

		public int ShutDownTimeMs{ get; set;}

		public SystemCallsStub ()
		{
			OnShutDown += delegate {};
		}

		#region ISystemCalls implementation

		public void Shutdown ()
		{
			System.Threading.Thread.Sleep (ShutDownTimeMs);
			OnShutDown ();
		}

		#endregion
	}
}

