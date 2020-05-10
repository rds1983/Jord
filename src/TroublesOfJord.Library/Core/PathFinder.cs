using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Priority_Queue;

namespace TroublesOfJord.Core
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

		private class PathGridNode: FastPriorityQueueNode
		{
			public PathGridNode Parent;
			public float F = float.MaxValue, G = float.MaxValue;
			public Point Position;
		};

		private readonly Point _start;
		private readonly Point _destination;
		private readonly Point _size;
		private readonly Func<Point, bool> _passableChecker;
		private readonly Func<Point, bool> _finishedChecker;
		private readonly Dictionary<int, PathGridNode> _nodes = new Dictionary<int, PathGridNode>();
		private readonly FastPriorityQueue<PathGridNode> _openSet;

		public Func<Point, Point, float> HeuristicFunction = HeuristicManhattan;

		public PathFinder(Point start, Point dest, Point size, 
			Func<Point, bool> passableChecker, Func<Point, bool> finishedChecker)
		{
			_start = start;
			_destination = dest;
			_size = size;
			_passableChecker = passableChecker;
			_finishedChecker = finishedChecker;

			_openSet = new FastPriorityQueue<PathGridNode>(_size.X * _size.Y);

			var startNode = GetNode(start);
			startNode.G = 0;
			startNode.F = HeuristicFunction(_start, _destination);
			startNode.Position = start;

			_openSet.Enqueue(startNode, startNode.F);
		}

		public ResultStep[] Process()
		{
			ResultStep[] result = null;

			while (_openSet.Count > 0)
			{
				var node = _openSet.Dequeue();

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
				for (var d = 0; d < 4; d++)
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

					var neighborNode = GetNode(newPos);
					
					float tentativeG = node.G;
					if (d < 4)
					{
						tentativeG += 1.0f;
					}
					else
					{
						tentativeG += 2.0f;
					}

					if (tentativeG < neighborNode.G)
					{
						neighborNode.Parent = node;
						neighborNode.G = tentativeG;
						neighborNode.F = neighborNode.G + HeuristicFunction(newPos, _destination);
						neighborNode.Position = newPos;

						if (!_openSet.Contains(neighborNode))
						{
							_openSet.Enqueue(neighborNode, neighborNode.F);
						}
						else
						{
							_openSet.UpdatePriority(neighborNode, neighborNode.F);
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
				result.Position = pos;
				_nodes[key] = result;
			}

			return result;
		}

		public static float HeuristicManhattan(Point a, Point b)
		{
			var dx = Math.Abs(b.X - a.X);
			var dy = Math.Abs(b.Y - a.Y);

			return dx + dy;
		}
	}
}