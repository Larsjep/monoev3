using System;

namespace StartupApp.WebServer
{
	public class MotorModel
	{
		public MotorModel ()
		{
		}

		public string Port { get; set; }
		public MonoBrickFirmware.Movement.Polarity Direction { get; set; }
		public int Speed { get; set; }
		public int Position { get; set; }
		public void Forward (int speed)
		{
			Position++;
			Speed = speed;
		}
		public void Reverse (int speed)
		{
			Position--;
			Speed = -speed;
		}
		public void Stop()
		{
			Speed = 0;
		}

		public void Break()
		{
			Speed = 0;
			Position = 0;
		}

		public void Move (int speed, int steps)
		{
			Speed = speed;
			Position = Position + steps;
		}

		public void Reset ()
		{
			Speed = 0;
			Position = 0;

		}


	}
}

