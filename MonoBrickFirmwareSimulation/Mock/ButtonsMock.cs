using System;
using MonoBrickFirmware.UserInput;
namespace MonoBrickFirmwareSimulation.Mock
{
	public class ButtonsMock : IButtons
	{


		public void EnterPressed(){}
		public void EnterReleased(){}
		public void DownPressed(){}
		public void DownReleased(){}
		public void UpPressed(){}
		public void UpReleased(){}
		public void LeftPressed(){}
		public void LeftReleased(){}
		public void RightPressed(){}
		public void RightReleased(){}

		public ButtonsMock ()
		{
		
		}

		public Buttons.ButtonStates GetStates ()
		{
			return 	Buttons.ButtonStates.None;	
		}

		public void WaitForKeyRelease (System.Threading.CancellationToken token)
		{
			
		}

		public void WaitForKeyRelease ()
		{
			
		}

		public Buttons.ButtonStates GetKeypress (System.Threading.CancellationToken token)
		{
			return 	Buttons.ButtonStates.None;	
		}

		public Buttons.ButtonStates GetKeypress ()
		{
			return 	Buttons.ButtonStates.None;		
		}

		public void LedPattern (int pattern)
		{
			
		}


	}
}

