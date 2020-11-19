namespace TroublesOfJord.Core
{
	public enum AttackType
	{
		Hit,
		Slash,
		Bash,
		Claw,
		Bite,
		Smash
	}

	public class AttackInfo
	{
		public AttackType AttackType;
		public int MinDamage;
		public int MaxDamage;

		public AttackInfo(AttackType attackType, int minDamage, int maxDamage)
		{
			AttackType = attackType;
			MinDamage = minDamage;
			MaxDamage = maxDamage;
		}
	}
}
