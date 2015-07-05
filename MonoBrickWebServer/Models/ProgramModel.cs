using System;
using MonoBrickFirmware.FileSystem;

namespace MonoBrickWebServer.Models
{
	public class ProgramModel : IProgramModel
	{
		private ProgramInformation programInfo;
		public ProgramModel (ProgramInformation programInfo)
		{
			this.programInfo = programInfo;

		}
		public string Name{ get {return programInfo.Name;}}
		public string ProgramLocation{ get {return LocationToString(programInfo.ProgramLocation);}}
		public bool IsAOTCompiled {get{return programInfo.IsAOTCompiled;}}
		public bool IsRunning{ get { return programInfo.IsRunning;} }

		public void Start()
		{
			ProgramManager.StartProgram(programInfo);
		}

		public void Stop()
		{
			ProgramManager.StopProgram(programInfo);
		}

		public void Delete ()
		{
			ProgramManager.DeleteProgram(programInfo);
		}

		private string LocationToString (ProgramLocation location)
		{
			string s = "";
			switch (location) 
			{
				case MonoBrickFirmware.FileSystem.ProgramLocation.ProgramFolder:
					s = "Program folder";
					break;
				case MonoBrickFirmware.FileSystem.ProgramLocation.SDCard:
					s = "SD card";
					break;
				case MonoBrickFirmware.FileSystem.ProgramLocation.Custom:
					s = "Custom";
					break;
			}
			return s;

		}
	}
}

