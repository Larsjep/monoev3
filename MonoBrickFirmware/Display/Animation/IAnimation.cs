using System;

namespace MonoBrickFirmware.Display.Animation
{
	public interface IAnimation
	{
		void Start();
		void Stop();
		bool IsRunning{get;}
	}
}

