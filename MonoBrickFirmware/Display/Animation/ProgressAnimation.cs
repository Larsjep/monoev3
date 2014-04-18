using System;
using System.Threading;
using System.Collections.Generic;
using MonoBrickFirmware.Display;

namespace MonoBrickFirmware.Display.Animation
{
	public class ProgressAnimation:IAnimation
	{
		private Thread thread;
		private ManualResetEvent resetEvent;
		private const int updateFrequency = 100;
		private const int rectSpace = 0;
		private const int rectWidth = 20;
		private int showIndex = 0;
		private Lcd lcd;
		private bool leftToRight = true;
		
		private List<Rectangle> rects = new List<Rectangle>();
		private bool run = false;
		
		public ProgressAnimation (Lcd lcd, Rectangle rect)
		{
			IsRunning = false;
			thread = new System.Threading.Thread (AnimationThread);
			int width = rect.P2.X - rect.P1.X;
			int numberOfRects = (width - rectSpace) / (rectWidth + rectSpace);
			int height = rect.P2.Y - rect.P1.Y;
			this.lcd = lcd;
			int totalWidthUsed = numberOfRects * (rectWidth + rectSpace);
			int startOffset = (width - totalWidthUsed)/2;
			Point animationPoint2 = new Point(rectWidth, height);
			Point spacePoint = new Point(rectSpace, 0);
			Point animationPoint = new Point(rectWidth,0);
			Point start = new Point(rect.P1.X + rectSpace + startOffset, rect.P1.Y);
			for (int i = 0; i < numberOfRects; i++) 
			{
				//Console.WriteLine("Start.X: " + start.X + " Start.Y: " + start.Y);
				//Console.WriteLine("Start+Ani.X: " + (start + animationPoint2).X + " Start+Ani.Y: " + (start + animationPoint2).Y);
				
				rects.Add( new Rectangle(start, start + animationPoint2) );
				start = start +  spacePoint + animationPoint;
			}
			resetEvent = new ManualResetEvent(false);
		}
		
		public void Start ()
		{
			if (!IsRunning) 
			{
				showIndex = 0;
				leftToRight = true;
				resetEvent.Reset();
				IsRunning = true;
				run = true;
				thread.Start();
			}	
		}
		
		public void Stop ()
		{
			run = false;
			resetEvent.Set ();
			thread.Join ();
			IsRunning = false;
		}
		
		private void AnimationThread ()
		{
			while (run) {
				for (int i = 0; i < rects.Count; i++) {
					lcd.DrawBox (rects [i], i == showIndex);
				}
				lcd.Update ();
				if (leftToRight) 
				{
					showIndex++;
					if(showIndex == rects.Count-1){
						leftToRight = false;
					}
				
				} 
				else 
				{
					showIndex--;
					if(showIndex == 0){
						leftToRight = true;
					}
				
				}
				resetEvent.WaitOne(updateFrequency);
			}
		
		}
		
		public bool IsRunning{get; private set;}
		
		
	}
}

