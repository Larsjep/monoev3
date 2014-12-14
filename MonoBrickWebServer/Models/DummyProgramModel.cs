using System;

namespace MonoBrickWebServer.Models
{
	public class DummyProgramModel : IProgramModel
	{
		private static bool running = false;
		public DummyProgramModel (string name)
		{
			this.Name = name;
		}

		public string Name{get;private set;}
		public string ProgramLocation{ get {return "Program folder";}}
		public bool IsAOTCompiled{ get {return false;}}
		public bool IsRunning{ get {return running;}}

		public void Start()
		{
			running = true;
		}

		public void Stop()
		{
			running = false;
		}

		public void Delete()
		{
			running = false;	
		}
	}
}

