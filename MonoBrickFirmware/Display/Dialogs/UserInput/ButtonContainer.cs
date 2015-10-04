using System;
using System.Collections.Generic;

namespace MonoBrickFirmware.Display.Dialogs.UserInput
{

	public abstract class ButtonContainer
	{
		private int x;
		private int y;
		private IButton selectedButton;
		private List<IButton> buttonsToRedraw = new List<IButton> ();
		protected int xMax;
		protected int yMax;
		protected bool updateEntireKeyboard = true;
		protected readonly IButton[,] buttons;
		protected Rectangle container;

		protected ButtonContainer (Rectangle container, int xSize, int ySize)
		{
			this.container = container;
			this.xMax = xSize;
			this.yMax = ySize;
			buttons = new IButton[xMax, yMax];
		}

		protected abstract void OnButtonSelected(IButton button);

		public abstract void Enter();

		public abstract void Esc ();

		public IButton SelectedButton
		{
			get{return selectedButton;} 
			protected set
			{ 
				if (selectedButton != null) 
				{
					selectedButton.Selected = false;
					buttonsToRedraw.Add (selectedButton);
				}

				selectedButton = value;
				selectedButton.Selected = true;

				buttonsToRedraw.Add (selectedButton);
			}
		}
		public virtual void Draw()
		{
			if (updateEntireKeyboard) 
			{
				DrawGrid();
				RefreshSelected ();
				updateEntireKeyboard = false;
			} 
			else 
			{
				RefreshSelected ();
			}
		}

		public void Up()
		{
			y = selectedButton.Position.Y -1;
			if (y < 0)
				y = yMax - 1;
			if (buttons [x, y].Disabled || buttons [x, y] == null)
			{
				SelectedButton = buttons[x,y];
				Up ();
			}
			UpdateXOnUpDown ();
			SelectedButton = buttons[x,y];
			OnButtonSelected (selectedButton);

		}
		public void Down()
		{
			y = selectedButton.Position.Y + selectedButton.Size.Y; 
			if (y >= yMax)
				y = 0;
			if (buttons [x, y].Disabled || buttons [x, y] == null)
			{
				SelectedButton = buttons[x,y];
				Down ();
			}
			UpdateXOnUpDown ();
			SelectedButton = buttons[x,y];
			OnButtonSelected (selectedButton);

		}

		public void Left()
		{
			x = selectedButton.Position.X-1;
			if (x < 0)
				x = xMax -1;
			if (buttons [x, y].Disabled || buttons [x, y] == null)
			{
				SelectedButton = buttons[x,y];
				Left ();
			}
			y = selectedButton.Position.Y; 
			SelectedButton = buttons[x,y];
			OnButtonSelected (selectedButton);

		}

		public void Right()
		{
			x = selectedButton.Position.X + selectedButton.Size.X;
			if (x >= xMax)
				x = 0;
			if (buttons [x, y].Disabled || buttons [x, y] == null)
			{
				SelectedButton = buttons[x,y];
				Right ();
			}
			y = selectedButton.Position.Y; 
			SelectedButton = buttons[x,y];
			OnButtonSelected (selectedButton);
		}

		protected void Add(IButton button)
		{
			for(int yPos = button.Position.Y; yPos < button.Position.Y + button.Size.Y; yPos++)
			{
				for(int xPos = button.Position.X; xPos < button.Position.X + button.Size.X; xPos++)
				{
					if (buttons [xPos, yPos] != null)
						throw new ArgumentException ("X,Y position already taken");
					buttons [xPos, yPos] = button;
				}			
			}
			updateEntireKeyboard = true;
		}

		private void DrawGrid()
		{
			for (int yPos = 0; yPos < yMax; yPos++)
			{
				for (int xPos = 0; xPos < xMax; xPos++)
				{
					buttons [xPos, yPos].Draw ();
				}
			}
		}


		private void RefreshSelected()
		{
			foreach (var button in buttonsToRedraw)
			{
				button.Draw ();
			}
			buttonsToRedraw.Clear ();
		}

		private void UpdateXOnUpDown()
		{
			switch (selectedButton.ExitType) 
			{
			case ExitType.Left:
				x = selectedButton.Position.X;
				break;
			case ExitType.Center:
				x = selectedButton.Position.X + selectedButton.Size.X / 2;
				break;
			case ExitType.Right:
				x = selectedButton.Position.X + selectedButton.Size.X-1;
				break;
			}
		}

	}
}

