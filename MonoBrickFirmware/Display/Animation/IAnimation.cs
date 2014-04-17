using System;

namespace MonoBrickFirmware
{
	public interface IAnimation
	{
		void Start();
		void Stop();
		bool IsRunning{get;}
	}
}

