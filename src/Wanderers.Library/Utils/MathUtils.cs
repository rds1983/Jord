using System;

namespace Wanderers.Utils
{
	public static class MathUtils
	{
		public static Random Random { get; } = new Random();

		public static int RollD20()
		{
			return Random.Next(1, 21);
		}
	}
}
