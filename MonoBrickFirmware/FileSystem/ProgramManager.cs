using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoBrickFirmware.Native;
using System.Threading;

namespace MonoBrickFirmware.FileSystem
{
	public class ProgramManager
	{
		private static readonly ProgramManager instance = new ProgramManager();
		private static string ProgramPathSdCard = "/mnt/bootpar/apps";
		private static string ProgramPathEV3 = "/home/root/apps/";
		private List<ProgramInformation> programList = new List<ProgramInformation>();

		private ProgramManager ()
		{


		}

		public static ProgramManager Instance
		{
			get{return instance;}
		}

		public void CreateSDCardFolder ()
		{
			if (!Directory.Exists (ProgramPathSdCard))
				Directory.CreateDirectory (ProgramPathSdCard);
		}

		public WaitHandle StartProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0)
		{
			ManualResetEvent waitHandle = new ManualResetEvent(false);
			lock (this) 
			{
				if (!program.IsRunning) 
				{
					(new Thread (() => {
						
						if (!runInAOT) {
							ProcessHelper.RunAndWaitForProcess ("/usr/local/bin/mono", program.ExeFile, timeout);
						} else {
							ProcessHelper.RunAndWaitForProcess ("/usr/local/bin/mono", "--full-aot " + program.ExeFile, timeout);
						}
						waitHandle.Set();
					})).Start ();
				}
			}
			return waitHandle;
		}

		public void StopProgram(ProgramInformation program)
		{
			lock (this) 
			{
				if(program.IsRunning) 
				{
					ProcessHelper.KillProcess(program.Name);
				}
			}
		}

		public void DeleteProgram(ProgramInformation program)
		{
			lock (this) 
			{
				if(program.IsRunning) 
				{
					ProcessHelper.KillProcess(program.Name);
				}
				Directory.Delete(program.Path,true);
			}
		}

		public bool AOTCompileProgram(ProgramInformation program)
		{
			bool ok = true;
			lock (this) 
			{
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
			}
			return ok;
		}

		public List<ProgramInformation> GetProgramInformationList()
		{
			lock (this) 
			{
				UpdateProgramList ();
				return programList;
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

