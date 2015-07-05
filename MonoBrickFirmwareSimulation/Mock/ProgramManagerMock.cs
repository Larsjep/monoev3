using System;
using MonoBrickFirmware.FileSystem;

namespace MonoBrickFirmwareSimulation.Mock
{
	public class ProgramManagerMock : IProgramManager 
	{
		public ProgramManagerMock ()
		{
		}

		#region IProgramManager implementation

		public void CreateSDCardFolder ()
		{
			
		}

		public int StartAndWaitForProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0)
		{
			return 0;
		}

		public void StartProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0)
		{
			
		}

		public void StopProgram (ProgramInformation program)
		{
			
		}

		public void DeleteProgram (ProgramInformation program)
		{
			
		}

		public bool AOTCompileProgram (ProgramInformation program)
		{
			return true;	
		}

		public System.Collections.Generic.List<ProgramInformation> GetProgramInformationList ()
		{
			return new System.Collections.Generic.List<ProgramInformation> ();
		}

		#endregion
	}
}

