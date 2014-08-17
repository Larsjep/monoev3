using System;
using System.IO;

namespace MonoBrickFirmware.Native
{
	public class AOTHelper
	{
		public static bool IsFileCompiled (string fileName)
		{
      		return File.Exists(fileName + ".so");
		}
		
		public static bool Compile(string fileName){
	      if (IsFileCompiled(fileName))
				File.Delete(new FileInfo(fileName).Name + ".so");
	      ProcessHelper.RunAndWaitForProcessWithOutput("mono", "--aot=full " + fileName);
	      return IsFileCompiled(fileName);
		}
	}
}

