using System;
using System.Collections.Generic;
using MonoBrickFirmware.Tools;

namespace MonoBrickFirmware.FileSystem
{
	public static class ProgramManager
	{
		static ProgramManager()
		{
			if(PlatFormHelper.RunningPlatform == PlatFormHelper.Platform.EV3)
			{
				Instance = new EV3ProgramManager();

			}
			else
			{
				Instance = null; //Not running on a EV3
			}
		}
		internal static IProgramManager Instance{ get; set; }
		public static void CreateSDCardFolder (){Instance.CreateSDCardFolder ();}
		public static void StartProgram (ProgramInformation program, bool runInAOT = false, Action<Exception> onExit = null){Instance.StartProgram (program, runInAOT, onExit);}
		public static void StopProgram (ProgramInformation program){Instance.StopProgram (program);}
		public static void DeleteProgram (ProgramInformation program){Instance.DeleteProgram (program);}
		public static bool AOTCompileProgram (ProgramInformation program){return Instance.AOTCompileProgram (program);}
		public static List<ProgramInformation> GetProgramInformationList (){return Instance.GetProgramInformationList ();}
		public static ProgramInformation RunningProgram{get{ return Instance.RunningProgram; }}

	}
}

