namespace Jord.Core.Items
{
	public class WeaponInfo : EquipInfo
	{
		public int MinDamage { get; set; }
		public int MaxDamage { get; set; }
		public AttackType AttackType { get; set; }

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