using DefaultEcs;
using Jord.Core.Items;
using Microsoft.Xna.Framework;
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

	public class CreatureInfo : BaseMapObject, ISpawnable
	{
		public string Name { get; set; }
		public int Experience { get; set; }
		public int Gold { get; set; }

		public int? MinimumLevel { get; set; }

		public int Occurence { get; set; } = 1;

		public CreatureType CreatureType { get; set; }

		public Inventory Inventory { get; set; }

		public List<AttackInfo> Attacks { get; } = new List<AttackInfo>();

		public int ArmorClass { get; set; }

		public int HitRoll { get; set; }

		public int MaxHp { get; set; }

		public int MaxMana { get; set; }

		public int MaxStamina { get; set; }

		public int HpRegen { get; set; }

		public List<LootInfo> Loot { get; } = new List<LootInfo>();

		public string DungeonFilter { get; set; }

		public CreatureInfo()
		{
			Inventory = new Inventory();
		}

		public Entity Spawn(Point location)
		{
			var result = TJ.World.CreateEntity();

			result.Set(new Location(location));
			result.Set(Image);
			result.Set(this);
			result.Set(Inventory.Clone());

			return result;
		}
	}
}