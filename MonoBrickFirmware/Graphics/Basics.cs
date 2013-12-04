using System;

namespace MonoBrickFirmware.Graphics
{
	public struct Point
	{
		public int X { get; set; }
		public int Y { get; set; }
		
		public Point (int x, int y) : this()
		{
			this.X = x;
			this.Y = y;
		}
		static public Point operator+(Point lhs, Point rhs)
		{
			return new Point(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}
		
		static public Point operator-(Point lhs, Point rhs)
		{
			return new Point(lhs.X - rhs.X, lhs.Y - rhs.Y);
		}
		
		static public Point operator*(Point p, int factor)
		{
			return new Point(p.X * factor, p.Y * factor);
		}
		
		static public Point operator*(Point p, double factor)
		{			
			return new Point((int)Math.Round(p.X * factor), (int)Math.Round (p.Y * factor));
		}				
	}
	
	public struct Rect
	{
		public Point P1 { get; set; }
		public Point P2 { get; set; }
		
		public Rect(Point p1, Point p2) : this()
		{
			this.P1 = p1;
			this.P2 = p2;
		}
		
		static public Rect operator+(Rect r, Point p)
		{
			return new Rect(r.P1 + p, r.P2 + p);
		}
			
	}
}

