namespace Jord.Core
{
	public sealed class LevelCost
	{
		public int Level { get; }
		public int Experience { get; }
		
		internal LevelCost(int level, int experience)
		{
			Level = level;
			Experience = experience;
		}
	}
}
