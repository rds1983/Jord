namespace TroublesOfJord.Core
{
	public static class Extensions
	{
		public static bool IsEnemy(this CreatureType type)
		{
			return type == CreatureType.Enemy ||
				type == CreatureType.NeutralEnemy;
		}

		public static bool IsNpc(this CreatureType type)
		{
			return !type.IsEnemy();
		}
	}
}
