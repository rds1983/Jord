using Microsoft.Xna.Framework;
using TroublesOfJord.Core;

namespace TroublesOfJord.Utils
{
	public static class CoreUtils
	{
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

		public static Point GetDelta(this MovementDirection dir)
		{
			return AllDirections[(int)dir];
		}

		public static TileInfo GetTileInfo(this Map map, int x, int y, TileInfo def)
		{
			if (x < 0 || x >= map.Width ||
				y < 0 || y >= map.Height)
			{
				return def;
			}

			return map[x, y].Info;
		}

		public static TileInfo GetTileInfo(this Map map, Point p, TileInfo def)
		{
			return GetTileInfo(map, p.X, p.Y, def);
		}
	}
}
