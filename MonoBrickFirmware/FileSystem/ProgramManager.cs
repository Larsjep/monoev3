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
		public static void StartProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0, Action onExit = null){Instance.StartProgram (program, runInAOT, timeout, onExit);}
		public static void StopProgram (ProgramInformation program){Instance.StopProgram (program);}
		public static void DeleteProgram (ProgramInformation program){Instance.DeleteProgram (program);}
		public static bool AOTCompileProgram (ProgramInformation program){return Instance.AOTCompileProgram (program);}
		public static List<ProgramInformation> GetProgramInformationList (){return Instance.GetProgramInformationList ();}
		public static ProgramInformation RunningProgram{get{ return Instance.RunningProgram; }}

	}
}

