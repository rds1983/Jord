using Jord.Loading;

namespace Jord.Core.Items
{
	public abstract class EquipInfo : BaseItemInfo
	{
		public int ArmorRating { get; set; }
	}

	public class ArmorInfo : EquipInfo
	{
		public override string BuildDescription()
		{
			return base.BuildDescription() + ", armor rating: " + ArmorRating;
		}
	}

	public class BasicArmorInfo : ArmorInfo
	{
		public EquipType SubType { get; set; }
	}

	public class ShieldInfo : ArmorInfo
	{
	}

	public enum WeaponType
	{
		OneHandedSword,
		OneHandedMace
	}

	public class WeaponInfo : EquipInfo
	{
		public WeaponType SubType { get; set; }
		public int MinDamage { get; set; }
		public int MaxDamage { get; set; }
		public AttackType AttackType { get; set; }

		public WeaponInfo()
		{
		}

		public override string BuildDescription()
		{
			return base.BuildDescription() + ", damage: " + MinDamage + "-" + MaxDamage;
		}
	}
}
