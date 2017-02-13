using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;

	using CommandLine;

	class Program
	{
		static void Main(string[] args)
		{
			var commandLineParser = Parser.Default.ParseArguments<Options>(args);
			bool exit = false;
			string mazePath = default(string);
		
			commandLineParser.WithParsed(
				p =>
				{
					
					mazePath = p.MazeMath;
					
				});

			commandLineParser.WithNotParsed(
				n =>
				{
					if (n.Any())
					{
						exit = true;
					}
				});

			if (exit)
			{
				return;
			}
			Maze maze;
			maze = mazePath.Length == 0 ? new Maze(100, 100) : new Maze(mazePath);

			var sw = Stopwatch.StartNew();
			var solver = new AStarPathFinder(maze);
			var path = solver.Solve();
			sw.Stop();
			Console.WriteLine($"Shortest path found in {sw.ElapsedMilliseconds}ms.  Total distance: {path.Count}");
			ImageDraw(maze, new List<Point>(), "maze.png");
			ImageDraw(maze,path, "maze-solved.png");

		}

		private static void ImageDraw(Maze maze, List<Point> path, string filename)
		{
			const int SCALE = 5;

			using (var image = new Bitmap(100 * SCALE, 100 * SCALE))
			{
				using (var g = Graphics.FromImage(image))
				{
					g.Clear(Color.Black);

					var openBrush = new SolidBrush(Color.White);
					var walkedBrush = new SolidBrush(Color.Green);

					for (int x = 0; x < maze.Width; x++)
					{
						for (int y = 0; y < maze.Height; y++)
						{
							if (maze[x, y] == Maze.State.Open) g.FillRectangle(openBrush, x * SCALE, y * SCALE, SCALE, SCALE);

							if (path.Contains(new Point(x, y)))
							{
								g.FillRectangle(walkedBrush, x * SCALE, y * SCALE, SCALE, SCALE);
							} 
						
							if (maze[x, y] == Maze.State.Start) g.FillRectangle(new SolidBrush(Color.Blue), x * SCALE, y * SCALE, SCALE, SCALE);
							if (maze[x, y] == Maze.State.End) g.FillRectangle(new SolidBrush(Color.Red), x * SCALE, y * SCALE, SCALE, SCALE);
						}
					}

					g.Save();
					File.Delete(filename);
					image.Save(filename, ImageFormat.Png);

				
				}
			}
		}



		class Options
		{
			
			[Option('f', "maze-file-path",
				 HelpText =
					 "Maze file path - Load a maze bmp image.  Use R:0,G:0,B:255 for start,  R:255,G:0,B:0 for end, R:255,G:255,B:255 for path and R:0,G:0,B:0 for closed",
				 Required = false)]
			public string MazeMath { get; set; } = string.Empty;

		}
	}
}
