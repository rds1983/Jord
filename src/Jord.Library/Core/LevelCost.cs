namespace Jord.Core
{
	public sealed class LevelCost
	{
		public int Level { get; }
		public int Experience { get; }
		public int Gold { get; }

		internal LevelCost(int level, int experience, int gold)
		{
			Level = level;
			Experience = experience;
			Gold = gold;
		}
	}
}
