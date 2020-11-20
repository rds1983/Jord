using System.Collections.Generic;

namespace TroublesOfJord.Core
{
	public enum CreatureType
	{
		Enemy,
		Merchant,
		Instructor
	}

	public class CreatureInfo : BaseMapObject
	{
		public string Name { get; set; }
		public int Experience { get; set; }
		public int Gold { get; set; }

		public int? MinimumLevel { get; set; }

		public CreatureType CreatureType;

		public Inventory Inventory { get; set; }

		public List<AttackInfo> Attacks { get; } = new List<AttackInfo>();

		public int ArmorClass;

		public int HitRoll;

		public int MaxHp, MaxMana, MaxStamina;
		public int HpRegen;

		public string DungeonFilter;

		public CreatureInfo()
		{
			Inventory = new Inventory();
		}
	}
}