using Microsoft.Xna.Framework;
using System;

namespace Wanderers.Utils
{
	public static class MathUtils
	{
		private static readonly Random _random = new Random();

		public static Random Random => _random;

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
