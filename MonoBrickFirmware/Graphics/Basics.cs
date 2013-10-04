using System;

namespace MonoBrickFirmware
{
	public struct Point
	{
		public int x { get; set; }
		public int y { get; set; }
		
		public Point (int x, int y) : this()
		{
			this.x = x;
			this.y = y;
		}
		static public Point operator+(Point lhs, Point rhs)
		{
			return new Point(lhs.x + rhs.x, lhs.y + rhs.y);
		}
		
		static public Point operator-(Point lhs, Point rhs)
		{
			return new Point(lhs.x - rhs.x, lhs.y - rhs.y);
		}
		
		static public Point operator*(Point p, int factor)
		{
			return new Point(p.x * factor, p.y * factor);
		}
		
		static public Point operator*(Point p, double factor)
		{			
			return new Point((int)Math.Round(p.x * factor), (int)Math.Round (p.y * factor));
		}				
	}
	
	public struct Rect
	{
		public Point p1 { get; set; }
		public Point p2 { get; set; }
		
		public Rect(Point p1, Point p2) : this()
		{
			this.p1 = p1;
			this.p2 = p2;
		}
		
		static public Rect operator+(Rect r, Point p)
		{
			return new Rect(r.p1 + p, r.p2 + p);
		}
			
	}
}

