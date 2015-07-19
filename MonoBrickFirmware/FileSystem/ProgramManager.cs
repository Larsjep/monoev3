using System;
using System.Collections.Generic;

namespace MonoBrickFirmware.FileSystem
{
	public static class ProgramManager
	{
		static ProgramManager()
		{
			try
			{
				Instance = new EV3ProgramManager();

			}
			catch
			{
				Instance = null; //Not running on a EV3
			}
		}
		internal static IProgramManager Instance{ get; set; }
		public static void CreateSDCardFolder (){Instance.CreateSDCardFolder ();}
		public static int StartAndWaitForProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0){return Instance.StartAndWaitForProgram (program, runInAOT, timeout);}
		public static void StartProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0){Instance.StartProgram (program, runInAOT, timeout);}
		public static void StopProgram (ProgramInformation program){Instance.StopProgram (program);}
		public static void DeleteProgram (ProgramInformation program){Instance.DeleteProgram (program);}
		public static bool AOTCompileProgram (ProgramInformation program){return Instance.AOTCompileProgram (program);}
		public static List<ProgramInformation> GetProgramInformationList (){return Instance.GetProgramInformationList ();}

	}
}

