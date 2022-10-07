namespace Jord.Core
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
		public AttackType AttackType { get; set; }
		public int MinDamage { get; set; }
		public int MaxDamage { get; set; }

		public AttackInfo(AttackType attackType, int minDamage, int maxDamage)
		{
			AttackType = attackType;
			MinDamage = minDamage;
			MaxDamage = maxDamage;
		}
	}
}
