using Microsoft.Xna.Framework;
using Wanderers.Core;
using Wanderers.Utils;

namespace Wanderers.Generation
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
					map.SetTileAt(x, y, new Tile(fillerTile));
				}
			}

			var roomsCount = MathUtils.Random.Next(MinimumRoomsCount, MaximumRoomsCount + 1);

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

							if (map.GetTileAt(x, y).Info == spaceTile)
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

				// Fill with space
				for (var x = location.X; x < location.X + size.X; ++x)
				{
					for (var y = location.Y; y < location.Y + size.Y; ++y)
					{
						if (x < 0 || x >= Width || y < 0 || y >= Height)
						{
							continue;
						}

						map.GetTileAt(x, y).Info = spaceTile;
					}
				}
			}

			return map;
		}
	}
}
