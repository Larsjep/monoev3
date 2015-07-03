using System;
using MonoBrickFirmware.Display;
using System.Threading;

namespace MonoBrickFirmware.Display.Menus
{
	public enum ItemSymbole {None, LeftArrow, RightArrow};
	
	public class ItemWithAction : IChildItem
	{
		
		private string text;
		private Action<CancellationToken> actionWithCancel = null;
		private ItemSymbole symbole;
		private const int arrowEdge = 4;
		private const int arrowOffset = 4;
		private CancellationTokenSource cancelSource = new CancellationTokenSource ();
		private bool hasFocus;
		private bool useEscToCancel;

		public ItemWithAction(string text, Action<CancellationToken> action, bool useEscToCancel, ItemSymbole symbole = ItemSymbole.None){
			this.text = text;
			this.actionWithCancel = action;
			this.symbole = symbole;
			this.useEscToCancel = useEscToCancel;
		}
		public IParentItem Parent{ get; set;}

		public void OnEnterPressed()
		{
			if (!hasFocus) 
			{
				hasFocus = true;
				Parent.SetFocus (this); 
				new Thread(
					delegate() {
						actionWithCancel (cancelSource.Token);
						Parent.RemoveFocus(this);
						hasFocus = false;
				}).Start();
			}
		}

		public void OnDrawTitle(Font f, Rectangle r, bool selected)
		{
			Lcd.WriteTextBox (f, r, text, selected);
			if (symbole == ItemSymbole.LeftArrow || symbole == ItemSymbole.RightArrow) {
				int arrowWidth =(int) f.maxWidth/3;
				Rectangle arrowRect = new Rectangle(new Point(r.P2.X -(arrowWidth+arrowOffset), r.P1.Y + arrowEdge), new Point(r.P2.X -arrowOffset, r.P2.Y-arrowEdge));
				if(symbole == ItemSymbole.LeftArrow)
					Lcd.DrawArrow(arrowRect,Lcd.ArrowOrientation.Left, selected);
				else
					Lcd.DrawArrow(arrowRect,Lcd.ArrowOrientation.Right, selected);
			}
		}

		public void OnEscPressed ()
		{
			if (useEscToCancel) 
			{
				OnHideContent ();	
			}
		}

		public void OnLeftPressed ()
		{

		}

		public void OnRightPressed()
		{

		}

		public void OnUpPressed ()
		{

		}

		public void OnDownPressed ()
		{

		}

		public void OnDrawContent ()
		{

		}

		public void OnHideContent()
		{
			cancelSource.Cancel ();	
		}
	}
}

