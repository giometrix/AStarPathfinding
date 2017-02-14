using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Drawing.Imaging;
	using System.IO;

	using CommandLine;

	using Gifed;

	class Program
	{
		const int SCALE = 5;

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
			Animate(path, "maze.png", "maze-solved.png", "animated.gif");
		}

		private static void Animate(List<Point> path, string mazeFilename, string mazeSolvedFilename, string outputFilename)
		{
			var gif = new AnimatedGif();
			gif.AddFrame(Image.FromFile(mazeFilename), 50);
			

			if (path.Count > 2)
			{
				using (var m = Bitmap.FromFile("mario.gif"))
				{
					for (int i = 0; i < path.Count; i++)
					{
						if (i % 10 == 0 || i == path.Count - 1)
						{
							var image = Bitmap.FromFile(mazeFilename);

							var g = Graphics.FromImage(image); //note: disposing breaks the animated gif save method.


							g.DrawImage(m, new Point(path[i].X * SCALE, path[i].Y * SCALE));

							if (i != path.Count - 1)
							{
								gif.AddFrame(image, 50);

							}
							else
							{
								gif.AddFrame(image, 100);
							}


						}

					}
				}
			}
			gif.AddFrame(Image.FromFile(mazeSolvedFilename),100);
			File.Delete(outputFilename);
			gif.Save(outputFilename);
		}

		private static void ImageDraw(Maze maze, List<Point> path, string filename)
		{

			
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

					using (var p = Bitmap.FromFile("princess.gif"))
					{
						g.DrawImage(p, new Point(maze.End.X * SCALE, maze.End.Y * SCALE));
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
