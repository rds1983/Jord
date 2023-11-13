using Jord.Core.Items;
using System.Collections.Generic;

namespace Jord.Core
{
	public enum CreatureType
	{
		Enemy,
		Merchant
	}

	public class LootInfo
	{
		public BaseItemInfo ItemInfo { get; set; }
		public int Rate { get; set; }
	}

	public class CreatureInfo : BaseMapObject
	{
		public string Name { get; set; }
		public int Experience { get; set; }
		public int Gold { get; set; }

		public int? MinimumLevel { get; set; }

		public int Occurence { get; set; } = 1;

		public CreatureType CreatureType { get; set; }

		public Inventory Inventory { get; set; }

		public List<AttackInfo> Attacks { get; } = new List<AttackInfo>();
		public int MeleeMastery { get; set; }

		public int ArmorClass { get; set; }

		public int EvasionRating { get; set; }
		public int BlockingRating { get; set; }

		public int MaxHp { get; set; }

		public int MaxMana { get; set; }
		
		public int MaxStamina { get; set; }

		public int HpRegen { get; set; }

		public List<LootInfo> Loot { get; } = new List<LootInfo>();

		public CreatureInfo()
		{
			Inventory = new Inventory();
		}
	}
}