namespace Wanderers.Core
{
	public enum AttackType
	{
		Hit,
		Slash,
		Claw
	}

	public class AttackInfo
	{
		public AttackType AttackType;
		public int MinDamage;
		public int MaxDamage;
		public int Delay;
	}
}
