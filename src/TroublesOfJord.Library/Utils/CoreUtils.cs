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
	}
}
