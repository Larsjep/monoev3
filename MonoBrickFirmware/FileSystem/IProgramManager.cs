using System.Collections.Generic;

namespace MonoBrickFirmware.FileSystem
{
	public interface IProgramManager
	{
		void CreateSDCardFolder ();
		int StartAndWaitForProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0);
		void StartProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0);
		void StopProgram (ProgramInformation program);
		void DeleteProgram (ProgramInformation program);
		bool AOTCompileProgram (ProgramInformation program);
		List<ProgramInformation> GetProgramInformationList ();
	}

}

