using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Wanderers.Core
{
	public class PathFinder
	{
		public struct ResultStep
		{
			public Point Position;
			public float Distance;
		}

		private static readonly float SquareRootFromTwo = (float)Math.Sqrt(2);
		private const int DirectionsCount = 8;

		public static readonly Point[] AllDirections = {
			new Point(-1, 0),
			new Point(1, 0),
			new Point(0, -1),
			new Point(0, 1),
			new Point(-1, -1),
			new Point(1, -1),
			new Point(-1, 1),
			new Point(1, 1),
		};

		private class PathGridNode
		{
			public PathGridNode Parent;
			public float F, G;
			public Point Position;
			public bool IsOpen;
			public bool IsClosed;
		};

		private readonly Point _start;
		private readonly Point _destination;
		private readonly Point _size;
		private readonly Func<Point, bool> _passableChecker;
		private readonly Func<Point, bool> _finishedChecker;
		private readonly Dictionary<int, PathGridNode> _nodes = new Dictionary<int, PathGridNode>();

		private readonly SortedDictionary<float, Stack<PathGridNode>> _openSet =
			new SortedDictionary<float, Stack<PathGridNode>>();

		public Point Start
		{
			get { return _start; }
		}

		public PathFinder(Point start, Point dest, Point size, 
			Func<Point, bool> passableChecker, Func<Point, bool> finishedChecker)
		{
			_start = start;
			_destination = dest;
			_size = size;
			_passableChecker = passableChecker;
			_finishedChecker = finishedChecker;

			var startNode = GetNode(start);
			startNode.G = 0;
			startNode.F = HeuristicFunction(_start, _destination);
			startNode.Position = start;
			AddToOpenSet(startNode);
		}

		public ResultStep[] Process()
		{
			ResultStep[] result = null;

			while (_openSet.Count > 0)
			{
				var node = PopFromOpenSet();

				if (_finishedChecker(node.Position))
				{
					var directionsCount = 0;
					var fNode = node;
					while (fNode.Position != _start)
					{
						fNode = fNode.Parent;
						directionsCount++;
					}

					// Initialize the result
					result = new ResultStep[directionsCount];

					// Fill it with data
					fNode = node;
					for (var i = 0; i < directionsCount; i++)
					{
						result[directionsCount - i - 1].Position = fNode.Position;
						result[directionsCount - i - 1].Distance = fNode.G;
						fNode = fNode.Parent;
					}

					break;
				}

				// Iterate through adjancent nodes
				for (var d = 0; d < DirectionsCount; d++)
				{
					var delta = AllDirections[d];
					var newPos = node.Position;
					newPos += delta;

					if (newPos.X < 0 || newPos.Y < 0 ||
						newPos.X >= _size.X || newPos.Y >= _size.Y)
					{
						continue; // Outside the map
					}

					if (!_passableChecker(newPos))
					{
						continue; // Not reacheable
					}

					float tentativeG = node.G;
					if (d < 4)
					{
						tentativeG += 1.0f;
					}
					else
					{
						tentativeG += SquareRootFromTwo;
					}

					var neighborNode = GetNode(newPos);
					if (neighborNode.IsClosed && tentativeG >= neighborNode.G)
					{
						continue;
					}

					if (!neighborNode.IsOpen || tentativeG < neighborNode.G)
					{
						neighborNode.Parent = node;
						neighborNode.G = tentativeG;
						neighborNode.F = neighborNode.G + HeuristicFunction(newPos, _destination);
						neighborNode.Position = newPos;
						if (!neighborNode.IsOpen)
						{
							AddToOpenSet(neighborNode);
						}
					}
				}
			}

			return result;
		}

		private int GetKey(Point pos)
		{
			return pos.Y * _size.X + pos.X;
		}

		private PathGridNode GetNode(Point pos)
		{
			var key = GetKey(pos);

			PathGridNode result;
			if (!_nodes.TryGetValue(key, out result))
			{
				result = new PathGridNode();
				_nodes[key] = result;
			}

			return result;
		}

		private void AddToOpenSet(PathGridNode node)
		{
			node.IsOpen = true;
			node.IsClosed = false;
			Stack<PathGridNode> nodes;
			if (!_openSet.TryGetValue(node.F, out nodes))
			{
				nodes = new Stack<PathGridNode>();
				_openSet[node.F] = nodes;
			}

			nodes.Push(node);
		}

		private PathGridNode PopFromOpenSet()
		{
			// Top node should have lowest F
			var first = _openSet.First();

			var node = first.Value.Pop();
			if (first.Value.Count == 0)
			{
				_openSet.Remove(first.Key);
			}

			node.IsOpen = false;
			node.IsClosed = true;

			return node;
		}

		private static float HeuristicFunction(Point start, Point end)
		{
			return 8.0f * (Math.Abs((float)(end.X - start.X)) * Math.Abs((float)(end.Y - start.Y)));
		}
	}
}