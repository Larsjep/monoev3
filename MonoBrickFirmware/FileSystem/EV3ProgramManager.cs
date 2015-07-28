using System;
using System.Collections.Generic;
using System.IO;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.FileSystem
{
	internal class EV3ProgramManager : IProgramManager
	{
		private string ProgramPathSdCard = "/mnt/bootpar/apps";
		private string ProgramPathEV3 = "/home/root/apps/";
		private List<ProgramInformation> programList = new List<ProgramInformation>();
		private Action<Exception> onExit = null;
		public void CreateSDCardFolder ()
		{
			if (!Directory.Exists (ProgramPathSdCard))
				Directory.CreateDirectory (ProgramPathSdCard);
		}

		public bool StartProgram(ProgramInformation program, bool runInAOT = false, int timeout = 0, Action<Exception> onExit = null)
		{
			this.onExit = onExit;
			if (!runInAOT)
			{
				RunningProgram = program;
				ProcessHelper.StartProcess("/usr/local/bin/mono", program.ExeFile, OnProgramCompleted);
			} 
			else 
			{
				RunningProgram = program;
				ProcessHelper.StartProcess("/usr/local/bin/mono", "--full-aot " + program.ExeFile, OnProgramCompleted);
			}
			return true;
		}

		public void StopProgram(ProgramInformation program)
		{
			if(RunningProgram != null && RunningProgram.Name == program.Name) 
			{
				ProcessHelper.KillProcess(program.Name);
			}

		}

		public void DeleteProgram(ProgramInformation program)
		{
			if(RunningProgram != null && RunningProgram.Name == program.Name) 
			{
				ProcessHelper.KillProcess(program.Name);
			}
			Directory.Delete(program.Path,true);

		}

		public bool AOTCompileProgram(ProgramInformation program)
		{
			bool ok = true;
			if (ProgramManager.RunningProgram == null) 
			{
				foreach (var file in program.DllFiles) 
				{
					if (!AOTHelper.Compile (System.IO.Path.Combine (program.Path, file))) 
					{
						ok = false;
						break;
					}
				}
				ok = AOTHelper.Compile (program.ExeFile);
			} 
			else 
			{
				ok = false;
			}
			return ok;
		}

		public List<ProgramInformation> GetProgramInformationList()
		{
			UpdateProgramList ();
			return programList;
		}

		public ProgramInformation RunningProgram{ get; private set;}

		private void OnProgramCompleted(int exitCode)
		{
			RunningProgram = null;
			if (onExit != null) 
			{
				Exception e = null;
				if (exitCode != 0)
				{
					e = new Exception ("Program exited with error code " + exitCode);
				}
				onExit(e);
			}
		}

		private void UpdateProgramList ()
		{
			var media = new string[]{ ProgramPathSdCard, ProgramPathEV3 };
			programList = new List<ProgramInformation> ();
			foreach (var mediaPath in media) {
				IEnumerable<string> programsFolders = Directory.EnumerateDirectories (mediaPath);
				foreach (var programFolder in programsFolders) {
					ProgramLocation location = programFolder.Contains (ProgramPathSdCard) ? ProgramLocation.SDCard : ProgramLocation.ProgramFolder; 
					programList.Add (new ProgramInformation (programFolder, location));
				}
			}

		}


	}
}

