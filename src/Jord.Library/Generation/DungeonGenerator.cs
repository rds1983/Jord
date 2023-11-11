using GoRogue.MapViews;
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
		private struct AdditionalTileData
		{
			public int? RoomIndex;
			public bool IsConnected;
		}

		private class RoomData
		{
			public Point Center;
			public bool IsConnected;
		}

		private class GenerationContext
		{
			public readonly Map Result;
			public readonly AdditionalTileData[,] AdditionalTileData;
			public readonly List<RoomData> Rooms = new List<RoomData>();

			public GenerationContext(int width, int height)
			{
				Result = new Map(width, height);
				AdditionalTileData = new AdditionalTileData[width, height];
			}

			public void DrawRoomSpace(int x, int y, int roomIndex, TileInfo info)
			{
				Result[x, y].Info = info;
				AdditionalTileData[x, y].RoomIndex = roomIndex;
			}
		}

		private GenerationContext _context;
		public const int RoomPadding = 3;

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

		private void DrawRoom(ref Rectangle roomRect)
		{
			var roomIndex = _context.Rooms.Count;
			_context.Rooms.Add(new RoomData
			{
				Center = roomRect.Center
			});

			if (MathUtils.RollPercentage(25))
			{
				// Simple rectangle room
				for (var x = roomRect.Left; x < roomRect.Right; ++x)
				{
					for (var y = roomRect.Top; y < roomRect.Bottom; ++y)
					{
						_context.DrawRoomSpace(x, y, roomIndex, Space);
					}
				}
			}
			else
			{
				// Overlapping room
				var center = roomRect.Center;

				var x1Left = -MathUtils.Random.Next(3, center.X - roomRect.X + 1) + center.X;
				var x1Right = MathUtils.Random.Next(3, roomRect.Right - center.X + 1) + center.X;
				var y1Top = -MathUtils.Random.Next(3, center.Y - roomRect.Y + 1) + center.Y;
				var y1Bottom = MathUtils.Random.Next(3, roomRect.Bottom - center.Y + 1) + center.Y;

				for (var x = x1Left; x < x1Right; ++x)
				{
					for (var y = y1Top; y < y1Bottom; ++y)
					{
						_context.DrawRoomSpace(x, y, roomIndex, Space);
					}
				}

				var x2Left = -MathUtils.Random.Next(3, center.X - roomRect.X + 1) + center.X;
				var x2Right = MathUtils.Random.Next(3, roomRect.Right - center.X + 1) + center.X;
				var y2Top = -MathUtils.Random.Next(3, center.Y - roomRect.Y + 1) + center.Y;
				var y2Bottom = MathUtils.Random.Next(3, roomRect.Bottom - center.Y + 1) + center.Y;

				for (var x = x2Left; x < x2Right; ++x)
				{
					for (var y = y2Top; y < y2Bottom; ++y)
					{
						_context.DrawRoomSpace(x, y, roomIndex, Space);
					}
				}

				var newRoomX = Math.Min(x1Left, x2Left);
				var newRoomY = Math.Min(y1Top, y2Top);
				var newRoomWidth = Math.Max(x1Right, x2Right) - newRoomX + 1;
				var newRoomHeight = Math.Max(y1Bottom, y2Bottom) - newRoomY + 1;

				roomRect = new Rectangle(newRoomX, newRoomY, newRoomWidth, newRoomHeight);
			}
		}

		private void Connect(Point source, Point target, int targetRoomIndex)
		{
			bool? isHorizontal = null;
			while(source != target) 
			{
				_context.Result[source.X, source.Y].Info = Space;
				_context.AdditionalTileData[source.X, source.Y].IsConnected = true;

				var atd = _context.AdditionalTileData[source.X, source.Y];
				if (atd.RoomIndex != null && !_context.Rooms[atd.RoomIndex.Value].IsConnected)
				{
					// We accidentially hit another unconnected room
					_context.Rooms[atd.RoomIndex.Value].IsConnected = true;
					if (atd.RoomIndex.Value == targetRoomIndex)
					{
						// Reached the target room
						return;
					}
				}

				var step = Point.Zero;
				if (source.X == target.X)
				{
					// We could go only vertically
					step.Y = target.Y < source.Y ? -1 : 1;
				} else if (source.Y == target.Y)
				{
					// Only horizontally
					step.X = target.X < source.X ? -1 : 1;
				} else
				{
					if (isHorizontal == null)
					{
						// 50% probability to go either vertical or horizontal
						isHorizontal = MathUtils.Random.Next(0, 2) == 0;
					} else
					{
						// 10% probability to change the orientation
						if (MathUtils.Random.Next(0, 10) == 0)
						{
							isHorizontal = !isHorizontal.Value;
						}
					}

					if (isHorizontal.Value)
					{
						step.X = target.X < source.X ? -1 : 1;
					} else
					{
						step.Y = target.Y < source.Y ? -1 : 1;
					}
				}

				source += step;
			}

			_context.Rooms[targetRoomIndex].IsConnected = true;
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
				{
					var roomRect = new Rectangle(roomX, roomY, roomWidth, roomHeight);
					DrawRoom(ref roomRect);
					paddedRoomsRects.Add(new Rectangle(
						roomRect.X - RoomPadding,
						roomRect.Y - RoomPadding,
						roomRect.Width + RoomPadding * 2,
						roomRect.Height + RoomPadding * 2));
				}
			}

			Step();

			// Add first room to connected rooms/points
			_context.Rooms[0].IsConnected = true;

			var sourceRoomIndex = 0;
			while(true)
			{
				// Check if all rooms are connected
				var hasUnconnectedRoom = false;
				foreach(var r in _context.Rooms)
				{
					if ( !r.IsConnected)
					{
						hasUnconnectedRoom |= true;
						break;
					}
				}

				if (!hasUnconnectedRoom)
				{
					break;
				}

				var source = _context.Rooms[sourceRoomIndex].Center;

				// Find closest room to the source
				int? targetRoomIndex = null;
				float dist = float.MaxValue;
				for(var i = 0; i < _context.Rooms.Count; ++i)
				{
					if (i == sourceRoomIndex || _context.Rooms[i].IsConnected)
					{
						continue;
					}

					var d = Vector2.DistanceSquared(source.ToVector2(), _context.Rooms[i].Center.ToVector2());
					if (targetRoomIndex == null || d < dist)
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
				var roomCenter2 = _context.Rooms[targetRoomIndex.Value].Center;
				for (var x = 0; x < Width; ++x)
				{
					for (var y = 0; y < Height; ++y)
					{
						if (!_context.AdditionalTileData[x, y].IsConnected)
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

				Connect(source, roomCenter2, targetRoomIndex.Value);

				Step();

				sourceRoomIndex = targetRoomIndex.Value;
			}

			return _context.Result;
		}
	}
}
