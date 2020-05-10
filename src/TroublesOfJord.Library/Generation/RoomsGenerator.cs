using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TroublesOfJord.Core;
using TroublesOfJord.Utils;

namespace TroublesOfJord.Generation
{
	public class RoomsGenerator: BaseGenerator
	{
		public int MinimumRoomsCount = 10;
		public int MaximumRoomsCount = 15;

		public int MinimumRoomWidth = 5;
		public int MaximumRoomWidth = 8;

		public int MinimumRoomHeight = 5;
		public int MaximumRoomHeight = 8;

		public int RoomBorderSize = 4;

		public string FillerTileId;
		public string SpaceTileId;

		private void AddToRoadTiles(HashSet<int> roadTiles, Point p)
		{
			var h = p.X + (p.Y * Width);

			roadTiles.Add(h);
		}

		private void Connect(Map map, TileInfo roadTile,
			List<Rectangle> rooms, HashSet<int> roadTiles, int sourceIndex, int destIndex)
		{
			if (sourceIndex == destIndex)
			{
				return;
			}

			var source = rooms[sourceIndex];
			var dest = rooms[destIndex];

			// Add dest entrances to road tiles
			AddToRoadTiles(roadTiles, source.Center);

			// Find closest location
			float? closestD = null;
			var destPos = Point.Zero;
			var startPos = Point.Zero;

			var dst = dest.Center;
			foreach (var h in roadTiles)
			{
				var p = new Point(h % Width, h / Height);
				var d = Vector2.Distance(p.ToVector2(), dst.ToVector2());

				if (closestD == null || closestD.Value > d)
				{
					closestD = d;
					startPos = dst;
					destPos = p;
				}
			}

			var pathFinder = new PathFinder(startPos,
				destPos,
				new Point(Width, Height),
				p => true,
				p => p == destPos);

			var steps = pathFinder.Process();

			if (steps == null || steps.Length == 0)
			{
				return;
			}

			foreach (var step in steps)
			{
				map[step.Position].Info = roadTile;
				AddToRoadTiles(roadTiles, step.Position);
			}
		}

		public override Map Generate()
		{
			var fillerTile = TJ.Module.EnsureTileInfo(FillerTileId);
			var spaceTile = TJ.Module.EnsureTileInfo(SpaceTileId);

			var map = new Map
			{
				Size = new Point(Width, Height)
			};

			for (var x = 0; x < Width; ++x)
			{
				for(var y = 0; y < Height; ++y)
				{
					map[x, y].Info = fillerTile;
				}
			}

			var roomsCount = MathUtils.Random.Next(MinimumRoomsCount, MaximumRoomsCount + 1);
			var rooms = new List<Rectangle>();
			for(var i = 0; i < roomsCount; ++i)
			{
				var size = new Point(MathUtils.Random.Next(MinimumRoomWidth, MaximumRoomWidth + 1),
					MathUtils.Random.Next(MinimumRoomHeight, MaximumRoomHeight + 1));

				var found = false;
				var location = Point.Zero;
				for (var attempt = 0; attempt < 1000 && !found; ++attempt)
				{
					location.X = MathUtils.Random.Next(1, Width - 1 - size.X);
					location.Y = MathUtils.Random.Next(1, Height - 1 - size.Y);

					// Make sure we dont intersect with another room
					found = true;
					for (var x = location.X - RoomBorderSize; x < location.X + size.X + RoomBorderSize; ++x)
					{
						for (var y = location.Y - RoomBorderSize; y < location.Y + size.Y + RoomBorderSize; ++y)
						{
							if (x < 0 || x >= Width || y < 0 || y >= Height)
							{
								continue;
							}

							if (map[x, y].Info == spaceTile)
							{
								found = false;
								goto finishAttempt;
							}
						}
					}
				finishAttempt:;
				}

				if (!found)
				{
					continue;
				}

				rooms.Add(new Rectangle(location.X, location.Y, size.X, size.Y));

				// Fill with space
				for (var x = location.X; x < location.X + size.X; ++x)
				{
					for (var y = location.Y; y < location.Y + size.Y; ++y)
					{
						if (x < 0 || x >= Width || y < 0 || y >= Height)
						{
							continue;
						}

						map[x, y].Info = spaceTile;
					}
				}
			}

			// Connect
			var roadTiles = new HashSet<int>();
			for(var i = 0; i < rooms.Count - 1; ++i)
			{
				Connect(map, spaceTile, rooms, roadTiles, i, i + 1);
			}

			return map;
		}
	}
}
