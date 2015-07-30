using System;

namespace MonoBrickFirmware.Native
{
	public static class SystemCalls
	{
		static SystemCalls(){Instance = new EV3SystemCalls ();}
		internal static ISystemCalls Instance { get; set;} 
		public static void ShutDown(){Instance.Shutdown ();}
	}
}

