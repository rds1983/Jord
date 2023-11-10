using GoRogue.MapViews;
using GoRogue.Pathing;
using GoRogue;
using Jord.Core;
using Jord.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.Diagnostics;

namespace Jord.Generation
{
	public class DungeonGenerator : BaseGenerator
	{
		private class GenerationContext
		{
			private class DungeonGeneratorView : IMapView<bool>
			{
				private readonly GenerationContext _context;

				public DungeonGeneratorView(GenerationContext context)
				{
					_context = context ?? throw new ArgumentNullException(nameof(context));
				}

				public bool this[Coord pos] => CheckPassable();

				public bool this[int index1D] => CheckPassable();

				public bool this[int x, int y] => CheckPassable();

				public int Height => _context.Result.Height;

				public int Width => _context.Result.Width;

				private static bool CheckPassable()
				{
					return MathUtils.Random.Next(0, 10) < 9;
				}
			}

			public readonly Map Result;
			public readonly List<Rectangle> RoomsRects = new List<Rectangle>();
			public readonly AStar PathFinder;
			public readonly HashSet<int> ConnectedRooms = new HashSet<int>();
			public readonly bool[,] ConnectedPoints;

			public GenerationContext(int width, int height)
			{
				Result = new Map(width, height);
				ConnectedPoints = new bool[width, height];
				var view = new DungeonGeneratorView(this);
				PathFinder = new AStar(view, Distance.MANHATTAN);
			}

			public void ConnectRoom(int roomIndex)
			{
				ConnectedRooms.Add(roomIndex);

				var roomRect = RoomsRects[roomIndex];
				for (var x = roomRect.Left; x < roomRect.Right; x++)
				{
					for (var y = roomRect.Top; y < roomRect.Bottom; y++)
					{
						ConnectedPoints[x, y] = true;
					}
				}
			}
		}

		private GenerationContext _context;
		public const int RoomPadding = 5;

		public TileInfo Space { get; set; }
		public TileInfo Wall { get; set; }

		public int MinimumRoomsCount { get; }
		public int MaximumRoomsCount { get; }
		public int MinimumRoomWidth { get; }
		public int MaximumRoomWidth { get; }
		public int MinimumRoomHeight { get; }
		public int MaximumRoomHeight { get; }

		public override Map CurrentResult => _context.Result;

		public DungeonGenerator(int width, int height,
				int minimumRoomsCount, int maximumRoomsCount,
				int minimumRoomWidth, int maximumRoomWidth,
				int minimumRoomHeight, int maximumRoomHeight) : base(width, height)
		{
			MinimumRoomsCount = minimumRoomsCount;
			MaximumRoomsCount = maximumRoomsCount;
			MinimumRoomWidth = minimumRoomWidth;
			MaximumRoomWidth = maximumRoomWidth;
			MinimumRoomHeight = minimumRoomHeight;
			MaximumRoomHeight = maximumRoomHeight;
		}

		private static void AddRectangle(bool[,] data, Rectangle rect)
		{
			for (var x = rect.Left; x < rect.Right; x++)
			{
				for (var y = rect.Top; y < rect.Bottom; y++)
				{
					data[x, y] = true;
				}
			}
		}

		private bool Connect(Point source, Point target, int targetRoomIndex)
		{
			if (source == target)
			{
				return true;
			}

			var path = _context.PathFinder.ShortestPath(source.ToCoord(), target.ToCoord());
			if (path == null)
			{
				return false;
			}

			foreach (var step in path.Steps)
			{
				_context.Result[step.X, step.Y].Info = Space;

				_context.ConnectedPoints[step.X, step.Y] = true;

				// We may accidentially hit another room
				for (var j = 0; j < _context.RoomsRects.Count; ++j)
				{
					if (_context.ConnectedRooms.Contains(j))
					{
						continue;
					}

					var r = _context.RoomsRects[j];
					var horizontalRect = new Rectangle(r.X - 1, r.Y, r.Width + 2, r.Height);
					var verticalRect = new Rectangle(r.X, r.Y - 1, r.Width, r.Height + 2);
					if (horizontalRect.Contains(step.ToPoint()) || verticalRect.Contains(step.ToPoint()))
					{
						AddRectangle(_context.ConnectedPoints, _context.RoomsRects[j]);
						_context.ConnectedRooms.Add(j);

						if (j == targetRoomIndex)
						{
							// Reached the target room
							return true;
						}
					}

				}
			}

			return true;
		}

		protected override Map InternalGenerate()
		{
			var roomsCount = MathUtils.Random.Next(MinimumRoomsCount, MaximumRoomsCount + 1);

			_context = new GenerationContext(Width, Height);
			var paddedRoomsRects = new List<Rectangle>();

			// Fill with walls
			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					_context.Result[x, y].Info = Wall;
				}
			}

			// Draw rooms
			for (var i = 0; i < roomsCount; i++)
			{
				var roomWidth = MathUtils.Random.Next(MinimumRoomWidth, MaximumRoomWidth);
				var roomHeight = MathUtils.Random.Next(MinimumRoomHeight, MaximumRoomHeight);

				int roomX = 0, roomY = 0;
				bool success = true;
				for (var tries = 100; tries > 0; tries--)
				{
					roomX = MathUtils.Random.Next(1, Width - roomWidth);
					roomY = MathUtils.Random.Next(1, Height - roomHeight);

					var roomRect = new Rectangle(roomX, roomY, roomWidth, roomHeight);

					// Make sure it doesnt intersect with any existing room
					success = true;

					for (var j = 0; j < paddedRoomsRects.Count; ++j)
					{
						if (Rectangle.Intersect(paddedRoomsRects[j], roomRect) != Rectangle.Empty)
						{
							success = false;
							break;
						}
					}

					if (success)
					{
						break;
					}
				}

				if (!success)
				{
					// Couldn't find a place for a room
					continue;
				}

				// Draw room
				for (var x = roomX; x < roomX + roomWidth; ++x)
				{
					for (var y = roomY; y < roomY + roomHeight; ++y)
					{
						_context.Result[x, y].Info = Space;
					}
				}


				_context.RoomsRects.Add(new Rectangle(roomX, roomY, roomWidth, roomHeight));
				paddedRoomsRects.Add(new Rectangle(
					roomX - RoomPadding,
					roomY - RoomPadding,
					roomWidth + RoomPadding * 2,
					roomHeight + RoomPadding * 2));
			}

			Step();

			// Add first room to connected rooms/points
			_context.ConnectRoom(0);

			var sourceRoomIndex = 0;
			while(true)
			{
				// Check if all rooms are connected
				if (_context.ConnectedRooms.Count == _context.RoomsRects.Count)
				{
					break;
				}

				var source = _context.RoomsRects[sourceRoomIndex].Center;
				// Find closest room to the source
				int? targetRoomIndex = null;
				float dist = float.MaxValue;
				for(var i = 0; i < _context.RoomsRects.Count; ++i)
				{
					if (i == sourceRoomIndex || _context.ConnectedRooms.Contains(i))
					{
						continue;
					}

					var d = Vector2.DistanceSquared(source.ToVector2(), _context.RoomsRects[i].Center.ToVector2());
					if (targetRoomIndex == null ||
						d < dist)
					{
						targetRoomIndex = i;
						dist = d;
					}
				}

				if (targetRoomIndex == null)
				{
					// Should never happen
					Debug.Assert(false);
					break;
				}

				// Now find the closest starting point
				dist = float.MaxValue;
				var roomCenter2 = _context.RoomsRects[targetRoomIndex.Value].Center;
				for (var x = 0; x < Width; ++x)
				{
					for (var y = 0; y < Height; ++y)
					{
						if (!_context.ConnectedPoints[x, y])
						{
							continue;
						}

						var d = Vector2.DistanceSquared(new Vector2(x, y),
							roomCenter2.ToVector2());

						if (d < dist)
						{
							source = new Point(x, y);
							dist = d;
						}
					}
				}

				// We reach the target in two searches: vertical and horizontal

				// Vertical search
				var target = new Point(source.X, roomCenter2.Y);
				if (!Connect(source, target, targetRoomIndex.Value))
				{
					continue;
				}

				Step();

				if (!_context.ConnectedRooms.Contains(targetRoomIndex.Value))
				{

					// Horizontal Search
					source = target;
					target = roomCenter2;
					Connect(source, target, targetRoomIndex.Value);

					Step();
				}

				sourceRoomIndex = targetRoomIndex.Value;
			}

			return _context.Result;
		}
	}
}
