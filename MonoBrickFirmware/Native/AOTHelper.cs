using System;
using System.IO;

namespace MonoBrickFirmware.Native
{
	internal class AOTHelper
	{
		public static bool IsFileCompiled (string fileName)
		{
      		return File.Exists(fileName + ".so");
		}
		
		public static bool Compile(string fileName){
	      if (IsFileCompiled(fileName))
				File.Delete(new FileInfo(fileName).Name + ".so");
	      ProcessHelper.RunAndWaitForProcessWithOutput("/usr/local/bin/mono", "--aot=full " + fileName);
	      return IsFileCompiled(fileName);
		}
	}
}

