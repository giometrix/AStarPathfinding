
namespace PathFinding
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;

	public class Node : IComparable
	{
		public Node(Point coordinates,float gScore, float hScore, Node parent)
		{
			this.Coordinates = coordinates;
			this.GScore = gScore;
			this.HScore = hScore;
			this.Parent = parent;
		}

		public Point Coordinates { get; }

		public float FScore => GScore + HScore;

		public float GScore { get; set; }

		public float HScore { get;  }

		public Node Parent { get; set; }

		public int CompareTo(object obj)
		{
			var node = obj as Node;
			if (node == null)
			{
				throw new ArgumentException();
			}
			return this.FScore.CompareTo(node.FScore);

		}
	}
}