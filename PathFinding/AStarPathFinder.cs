using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
	using System.Collections.Specialized;
	using System.Drawing;

	public class AStarPathFinder
	{
		private readonly Maze maze;

		private readonly List<Node> openList;

		private readonly Dictionary<Point, bool> openStatus;

		public AStarPathFinder(Maze maze)
		{
			this.maze = maze;
			this.openStatus = new Dictionary<Point, bool>();
			this.openList = new List<Node>();
		}

		private float CalculateManhattanDistance(Point coordinates)
		{
			return Math.Abs(this.maze.End.X - coordinates.X) + Math.Abs(this.maze.End.Y - coordinates.Y);
		}

		public List<Point> Solve()
		{
			this.openList.Add(new Node(this.maze.Start, 0, CalculateManhattanDistance(this.maze.Start), null));
		

			do
			{
				var current = this.openList.First();
				this.openStatus[current.Coordinates] = false;
				this.openList.Remove(current);
				if (current.Coordinates == this.maze.End)
				{
					var path = new List<Point>();
					var c = current;
					while (c != null)
					{
						path.Add(c.Coordinates);
						c = c.Parent;
					}
					path.Reverse();
					return path;

				}
				GetNeighbors(current);
			}
			while (this.openList.Count > 0);

			throw new Exception("No path found");
		}

		private void GetNeighbors(Node current)
		{
			
		
			var up = this.maze.MoveUp(current.Coordinates);
			var left = this.maze.MoveLeft(current.Coordinates);
			var down = this.maze.MoveDown(current.Coordinates);
			var right = this.maze.MoveRight(current.Coordinates);

			this.HandleNeighbor(current, up);
			this.HandleNeighbor(current, left);
			this.HandleNeighbor(current, down);
			this.HandleNeighbor(current, right);
		}

		private void HandleNeighbor(Node current, Point coordinates)
		{
			if (this.IsTraversable(coordinates))
			{
				if (this.IsOpen(coordinates))
				{
					Node node;
					if (this.NodeExists(coordinates))
					{
						node = this.GetNode(coordinates);
						var cost = current.GScore + 1;
						if (node.GScore > cost)
						{
							node.GScore = cost;
							node.Parent = current;
							this.openList.Sort();
						}
					}
					else
					{
						node = new Node(coordinates, current.GScore + 1, this.CalculateManhattanDistance(coordinates), current);
						this.openList.Add(node);
						this.openList.Sort();
						this.openStatus[coordinates] = true;
					}
				}
			}
		}

		private Node GetNode(Point coordinates)
		{
			return this.openList.Single(n => n.Coordinates == coordinates);
		}

		private bool NodeExists(Point coordinates)
		{
			return this.openStatus.ContainsKey(coordinates);
		}
		private bool IsOpen(Point coordinates)
		{
			return  !this.openStatus.ContainsKey(coordinates) || this.openStatus[coordinates];
		}

		private bool IsTraversable(Point nextCoordinates)
		{
			return this.maze[nextCoordinates.X, nextCoordinates.Y] != Maze.State.Closed;
		}
	}
}
