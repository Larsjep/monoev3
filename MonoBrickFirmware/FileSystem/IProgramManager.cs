using System.Collections.Generic;
using System;

namespace MonoBrickFirmware.FileSystem
{
	public interface IProgramManager
	{
		void CreateSDCardFolder ();
		bool StartProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0, Action onCompleted = null);
		void StopProgram (ProgramInformation program);
		void DeleteProgram (ProgramInformation program);
		bool AOTCompileProgram (ProgramInformation program);
		List<ProgramInformation> GetProgramInformationList ();
		ProgramInformation RunningProgram{ get;}
	}

}

