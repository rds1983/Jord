namespace Jord.Core.Items
{
	public class WeaponInfo : EquipInfo
	{
		public int MinDamage;
		public int MaxDamage;
		public AttackType AttackType;

		public WeaponInfo()
		{
			SubType = EquipType.Weapon;
		}

		public override string BuildDescription()
		{
			return base.BuildDescription() + ", damage: " + MinDamage + "-" + MaxDamage;
		}
	}
}