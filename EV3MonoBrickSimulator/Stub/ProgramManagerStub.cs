using System;
using MonoBrickFirmware.FileSystem;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace EV3MonoBrickSimulator.Stub
{
	public class ProgramManagerStub : IProgramManager 
	{

		private ManualResetEvent programDone = new ManualResetEvent(false);
		private Thread executionThread = null;
		private object programLock = new object();
		private List<ProgramInformation> programList = new List<ProgramInformation>();
		private string ProgramPathEV3;
		private Action onExit;
		public int AOTCompileTimeMs{ get; set;}
		public ProgramManagerStub ()
		{
			executionThread = new Thread (new ThreadStart (ProgramExecution));
			ProgramPathEV3 = Path.Combine (Directory.GetCurrentDirectory (), "Apps");
			if(!Directory.Exists(ProgramPathEV3))
			{
				Directory.CreateDirectory(ProgramPathEV3);
			}
		}

		#region IProgramManager implementation

		public void CreateSDCardFolder ()
		{
			//Do nothing 
		}

		public bool StartProgram (ProgramInformation program, bool runInAOT = false, int timeout = 0, Action onExit = null)
		{
			if (RunningProgram != null)
				return false;
			lock (programLock)
			{
				if (LoadDll (program)) {
					executionThread = new Thread (new ThreadStart (ProgramExecution));
					RunningProgram = program;
					this.onExit = onExit;
				} 
				else 
				{
					return false;	
				}
			}
			executionThread.Start ();
			return true;
		}

		public void StopProgram (ProgramInformation program)
		{
			executionThread.Abort ();
			executionThread.Join ();
		}

		public void DeleteProgram (ProgramInformation program)
		{
			Directory.Delete(program.Path,true);
		}

		public bool AOTCompileProgram (ProgramInformation program)
		{
			foreach (var file in program.DllFiles) 
			{
				File.Create(System.IO.Path.Combine (program.Path, file + ".so")).Close();
				System.Threading.Thread.Sleep (AOTCompileTimeMs);
			}
			File.Create(System.IO.Path.Combine (program.Path,  program.ExeFile + ".so")).Close();
			System.Threading.Thread.Sleep (AOTCompileTimeMs);
			return true;		
		}

		public List<ProgramInformation> GetProgramInformationList ()
		{
			UpdateProgramList ();
			return programList;
		}

		public ProgramInformation RunningProgram { get; private set;}

		private void UpdateProgramList ()
		{
			var media = new string[]{ProgramPathEV3 };
			programList = new List<ProgramInformation> ();
			foreach (var mediaPath in media) {
				IEnumerable<string> programsFolders = Directory.EnumerateDirectories (mediaPath);
				foreach (var programFolder in programsFolders) {
					ProgramLocation location = ProgramLocation.ProgramFolder; 
					programList.Add (new ProgramInformation (programFolder, location));
				}
			}

		}


		private void ProgramExecution()
		{
			try
			{
				AppDomain.CurrentDomain.ExecuteAssembly(RunningProgram.ExeFile);
			}
			catch(Exception e)
			{
				Console.WriteLine (e.Message);
				Console.WriteLine (e.StackTrace);
			}
			lock (programLock)
			{
				RunningProgram = null;
				if (onExit != null)
				{
					onExit ();
				}
			}
			programDone.Set ();

		}

		private bool LoadDll(ProgramInformation program)
		{
			bool ok = true;
			foreach (var dll in program.DllFiles) 
			{
				//Needs to be tested
				if(!dll.EndsWith("MonoBrickFirmware.dll"))
				{
					try
					{
						AppDomain.CurrentDomain.Load (dll);
					}
					catch
					{
						ok = false;
						break;
					}
				}
			}
			return ok;
		}


		#endregion
	}
}



