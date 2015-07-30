using System;
using System.Threading;

namespace MonoBrickFirmware.Native
{
	internal class EV3SystemCalls: ISystemCalls
	{
		public void Shutdown()
		{
			ProcessHelper.RunAndWaitForProcess ("/sbin/shutdown", "-h now");
			Thread.Sleep (120000);

		}
	}
}

