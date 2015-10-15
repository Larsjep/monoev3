using System;
using System.Threading;

namespace MonoBrickFirmware.Native
{
	public static class GacHelper
	{
		public static bool IsAssembyInGac(string assembly)
		{
			string output = ProcessHelper.RunAndWaitForProcessWithOutput ("gacutil", "/l " + assembly);
			var outputStrings = output.Split (null);
			bool installed = false;
			for (int i = 0; i < outputStrings.Length; i++)
			{
				if(outputStrings[i].ToLower().Contains("items") && outputStrings[i+1].ToLower().Contains("="))
				{
					installed = outputStrings[i+2] != "0";
					break;
				}
			
			}
			return installed;
		}

		public static bool AddAssembly(string assembly)
		{
			if(!assembly.ToLower().EndsWith(".dll"))
			{
				assembly = assembly + ".dll";		
			}
			string output = ProcessHelper.RunAndWaitForProcessWithOutput ("gacutil", "/i " + assembly);
			var outputStrings = output.Split (null);
			bool ok = outputStrings[0].ToLower() == "installed";
			return ok;	
		}
	}
}

