using Microsoft.Xna.Framework;
using System;

namespace TroublesOfJord.Utils
{
	public static class MathUtils
	{
		public static Random Random { get; } = new Random();

		public static int RollD20()
		{
			return Random.Next(1, 21);
		}

		public static bool RollPercentage(int percentage)
		{
			if (percentage < 1)
			{
				// Fail
				return false;
			}

			if (percentage >= 100)
			{
				// Win
				return true;
			}

			var rnd = Random.Next(1, 101);
			return rnd < percentage;
		}

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
