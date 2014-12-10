using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoBrickFirmware.Native;

namespace MonoBrickFirmware.FileSystem
{
	public enum ProgramLocation {ProgramFolder = 0, SDCard = 1, Custom = 2};

	public class EV3Program
	{
		private string exeFile;
		private static string ProgramPathSdCard = "/mnt/bootpar/apps";
		private static string ProgramPathEV3 = "/home/root/apps/";


		public EV3Program(string name, string path, string exeFile, ProgramLocation location, List<string> dllFiles)
		{
			Name = name;
			Path = path;
			this.exeFile = exeFile;
			this.ProgramLocation = location;
			DllFiles = dllFiles;
		}
		public string Name{get; private set;}
		public string Path;
		public ProgramLocation ProgramLocation{get; private set;}
		public List<string> DllFiles{get; private set;}

		public int Run(bool runInAOT = false, int timeout = 0)
		{
			if (!runInAOT) 
			{
				return ProcessHelper.RunAndWaitForProcess ("/usr/local/bin/mono", exeFile, timeout);
			}
			return ProcessHelper.RunAndWaitForProcess("/usr/local/bin/mono", "--full-aot " + exeFile, timeout);
		}

		public void Delete()
		{
			Directory.Delete(this.Path,true);
		}

		public bool AOTCompile()
		{
			bool ok = true;
			foreach (var file in DllFiles) 
			{
				if(!AOTHelper.Compile(System.IO.Path.Combine(this.Path, file)))
				{
					ok = false;
					break;
				}
			}
			ok = AOTHelper.Compile(exeFile);
			return ok;
		}

		public bool IsAOTCompiled 
		{
			get 
			{
				bool ok = true;
				foreach (var file in DllFiles) {
					if (!AOTHelper.IsFileCompiled (System.IO.Path.Combine (this.Path, file))) {
						ok = false;
						break;
					}
				}
				ok = AOTHelper.IsFileCompiled (exeFile);
				return ok;
			}
		}

		public bool IsRunning{ get{ return ProcessHelper.IsProcessRunning(exeFile);}}
		



		private static EV3Program CreateProgram (string programFolder, ProgramLocation location)
		{
			EV3Program program = null;
			try 
			{
				string name = programFolder; 
				string path =  new DirectoryInfo(programFolder).Name;
				IEnumerable<string> files = Directory.EnumerateFiles (programFolder);
				List<string> dllFiles = files.Where(s => s.EndsWith(".dll")).ToList();
				string exeFile = files.First(f => f.EndsWith("exe"));
				string exePath = System.IO.Path.Combine(programFolder, exeFile);  
				program = new EV3Program(name, path, exePath, location, dllFiles);
			} 
			catch 
			{
				
			}
			return program;	
		}

		public static EV3Program CreateProgram (string programFolder)
		{
			return CreateProgram(programFolder, ProgramLocation.Custom);		
		}


		public static void CreateSDCardFolder ()
		{
			if (!Directory.Exists (ProgramPathSdCard))
				Directory.CreateDirectory (ProgramPathSdCard);
			
		}

		public static List<EV3Program> GetProgramList()
		{
			var media = new string[]{ ProgramPathSdCard, ProgramPathEV3 };
			List<EV3Program> programList = new List<EV3Program>();
			foreach (var mediaPath in media) 
			{
				IEnumerable<string> programsFolders = Directory.EnumerateDirectories (mediaPath);
				foreach (var programFolder in programsFolders) 
				{
					ProgramLocation location = programFolder.Contains(ProgramPathSdCard) ? ProgramLocation.SDCard : ProgramLocation.ProgramFolder; 
					programList.Add(CreateProgram(programFolder, location));
				}
			}
			return programList;
		}


	}
}

