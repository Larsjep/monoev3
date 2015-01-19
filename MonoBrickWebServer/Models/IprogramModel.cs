using System;

namespace MonoBrickWebServer.Models
{
	public interface IProgramModel
	{
		string Name{get;}
		string ProgramLocation{get;}
		bool IsAOTCompiled{get;}
		bool IsRunning{get;}
		void Start();
		void Stop();
		void Delete();
	}
}

