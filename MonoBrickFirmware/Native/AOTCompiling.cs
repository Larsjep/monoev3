using System;

namespace MonoBrickFirmware.Native
{
	public class AOTCompiling
	{
		public static bool IsFileCompiled (string fileName)
		{
			return false;
		}
		
		public static void Compile(string fileName){
			//Do something here	
			System.Threading.Thread.Sleep(3000);
		}
	}
}

