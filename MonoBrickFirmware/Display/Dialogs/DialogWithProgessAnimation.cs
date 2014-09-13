using System;
using MonoBrickFirmware.Display.Animation;


namespace MonoBrickFirmware.Display.Dialogs
{
	public abstract class DialogWithProgessAnimation : Dialog
	{
		private ProgressAnimation progress = null;
		private const int progressEdgeX = 10;
		private const int progressEdgeY = 8;
		private int animationLine = -1;

		public DialogWithProgessAnimation (Font f, string title, int width = 160, int height = 90, int topOffset = 0): base(f,title,width, height, topOffset)
		{
			
		}

		protected void StartProgressAnimation (int lineIndex)
		{
			if (progress != null || lineIndex != animationLine) 
			{
				Rectangle progressRect = lines [lineIndex];
				animationLine = lineIndex;
				progressRect = new Rectangle (progressRect.P1 + new Point (progressEdgeX, progressEdgeY), progressRect.P2 + new Point (-progressEdgeX, -progressEdgeY));
				if (progress != null && progress.IsRunning)
					progress.Stop ();
				progress = new ProgressAnimation (progressRect);
			}
			progress.Start();
		}
		
		protected void StopProgressAnimation ()
		{
			progress.Stop();
		}
	}
}

