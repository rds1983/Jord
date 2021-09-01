using Microsoft.Xna.Framework;
using Jord.Core;
using GoRogue;

namespace Jord.Utils
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

		public static Point GetDelta(this MovementDirection dir) => AllDirections[(int)dir];

		public static Vector2 ToVector(this Point p) => new Vector2(p.X, p.Y);

		public static Coord ToCoord(this Point p) => new Coord(p.X, p.Y);
		public static Point ToPoint(this Coord c) => new Point(c.X, c.Y);
	}
}
