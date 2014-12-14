using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoBrickFirmware.Native;
using System.Threading;
namespace MonoBrickFirmware.FileSystem
{
	public enum ProgramLocation {ProgramFolder = 0, SDCard = 1, Custom = 2};

	public class ProgramInformation
	{
		public ProgramInformation (string programFolder) : this(programFolder, ProgramLocation.Custom)
		{
			
		}

		internal ProgramInformation (string programFolder, ProgramLocation location)
		{
			Name = programFolder; 
			ProgramLocation = location;
			Path =  new DirectoryInfo(programFolder).Name;
			IEnumerable<string> files = Directory.EnumerateFiles (programFolder);
			DllFiles = files.Where(s => s.EndsWith(".dll")).ToList();
			ExeFile = System.IO.Path.Combine(programFolder, files.First(f => f.EndsWith("exe")));  

		}

		public string Name{get; private set;}
		public ProgramLocation ProgramLocation{get; private set;}
		internal string Path{ get; private set;}
		internal List<string> DllFiles{get; private set;}
		internal string ExeFile{get; private set;}

		public bool IsAOTCompiled {
			get 
			{
				bool ok = true;
				foreach (var file in DllFiles) {
					if (!AOTHelper.IsFileCompiled (System.IO.Path.Combine (this.Path, file))) {
						ok = false;
						break;
					}
				}
				ok = AOTHelper.IsFileCompiled (ExeFile);
				return ok;
			}
		}

		public bool IsRunning 
		{
			get {return ProcessHelper.IsProcessRunning (ExeFile);}
		}

	}
}

