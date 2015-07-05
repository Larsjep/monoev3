using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoBrickFirmware.Native;
using System.Threading;

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

	public static class ProgramManager
	{
		static ProgramManager()
		{
			try
			{
				Instance = new EV3ProgramManager();

			}
			catch(Exception e)
			{
				Instance = null; //Not running on a EV3
			}
		}
		public static IProgramManager Instance{ get; set; }
		public static void CreateSDCardFolder (){Instance.CreateSDCardFolder ();}
		public static int StartAndWaitForProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0){return Instance.StartAndWaitForProgram (program, runInAOT, timeout);}
		public static void StartProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0){Instance.StartProgram (program, runInAOT, timeout);}
		public static void StopProgram (ProgramInformation program){Instance.StopProgram (program);}
		public static void DeleteProgram (ProgramInformation program){Instance.DeleteProgram (program);}
		public static bool AOTCompileProgram (ProgramInformation program){return Instance.AOTCompileProgram (program);}
		public static List<ProgramInformation> GetProgramInformationList (){return Instance.GetProgramInformationList ();}

	}


	public class EV3ProgramManager : IProgramManager
	{
		private string ProgramPathSdCard = "/mnt/bootpar/apps";
		private string ProgramPathEV3 = "/home/root/apps/";
		private List<ProgramInformation> programList = new List<ProgramInformation>();

		public void CreateSDCardFolder ()
		{
			if (!Directory.Exists (ProgramPathSdCard))
				Directory.CreateDirectory (ProgramPathSdCard);
		}

		public int StartAndWaitForProgram(ProgramInformation program, bool runInAOT = false, int timeout = 0)
		{
			if (!runInAOT) {
				return ProcessHelper.RunAndWaitForProcess ("/usr/local/bin/mono", program.ExeFile, timeout);
			} else {
				return ProcessHelper.RunAndWaitForProcess ("/usr/local/bin/mono", "--full-aot " + program.ExeFile, timeout);
			}
		}

		public void StartProgram(ProgramInformation program, bool runInAOT = false, int timeout = 0)
		{
			if (!runInAOT)
			{
				ProcessHelper.StartProcess("/usr/local/bin/mono", program.ExeFile);
			} 
			else 
			{
				ProcessHelper.StartProcess("/usr/local/bin/mono", "--full-aot " + program.ExeFile);
			}
		}


		public void StopProgram(ProgramInformation program)
		{
			if(program.IsRunning) 
			{
				ProcessHelper.KillProcess(program.Name);
			}

		}

		public void DeleteProgram(ProgramInformation program)
		{
			if(program.IsRunning) 
			{
				ProcessHelper.KillProcess(program.Name);
			}
			Directory.Delete(program.Path,true);

		}

		public bool AOTCompileProgram(ProgramInformation program)
		{
			bool ok = true;
			if (!program.IsRunning) 
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

