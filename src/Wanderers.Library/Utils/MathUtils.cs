using Microsoft.Xna.Framework;

namespace Wanderers.Utils
{
	public static class MathUtils
	{
		public static Vector2 ToVector2(this Point p)
		{
			return new Vector2(p.X, p.Y);
		}

		public static Point ToPoint(this Vector2 v)
		{
			return new Point((int)v.X, (int)v.Y);
		}
	}
}
