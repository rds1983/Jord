using Newtonsoft.Json.Linq;
using Jord.Core;
using Jord.Core.Items;
using System;
using System.Collections.Generic;

namespace Jord.Loading
{
	class CreatureInfoLoader: BaseMapObjectLoader<CreatureInfo>
	{
		public static readonly CreatureInfoLoader Instance = new CreatureInfoLoader();

		protected override CreatureInfo CreateObject(string source, JObject data, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			var result = new CreatureInfo
			{
				Name = data.EnsureString("Name"),
				CreatureType = data.EnsureEnum<CreatureType>("Type"),
				MinimumLevel = data.OptionalNullableInt("MinimumLevel"),
				Occurence = data.OptionalInt("Occurence", 1)
			};

			string dungeonFilter;
			if (properties != null && properties.TryGetValue("DungeonFilter", out dungeonFilter))
			{
				result.DungeonFilter = dungeonFilter;
			}

			result.Gold = data.OptionalInt("Gold");

			if (result.CreatureType == CreatureType.Enemy)
			{
				result.Experience = data.EnsureInt("Experience");
			}

			if (result.CreatureType == CreatureType.Enemy)
			{
				result.ArmorClass = data.EnsureInt("ArmorClass");
				result.HitRoll = data.EnsureInt("HitRoll");
				result.MaxHp = data.EnsureInt("MaxHp");
				result.HpRegen = data.OptionalInt("HpRegen", Constants.DefaultHpRegen);
				result.MaxMana = data.OptionalInt("MaxMana");
				result.MaxStamina = data.OptionalInt("MaxStamina");

				var attacks = (JArray)data["Attacks"];
				foreach(JObject attackObj in attacks)
				{
					result.Attacks.Add(attackObj.ParseAttack());
				}
			}

			secondRunAction = db => SecondRun(result, data, db);

			return result;
		}

		private void SecondRun(CreatureInfo result, JObject data, Database database)
		{
			var inventoryObj = data.OptionalJObject("Inventory");
			if (inventoryObj != null)
			{
				var inventory = new Inventory();

				foreach (var pair in inventoryObj)
				{
					inventory.Items.Add(new ItemPile
					{
						Item = new Item(database.ItemInfos.Ensure(pair.Key)),
						Quantity = (int)pair.Value
					});
				}

				result.Inventory = inventory;
			}

			var lootArray = data.OptionalJArray("Loot");
			if (lootArray != null)
			{
				foreach (JObject lootObj in lootArray)
				{
					result.Loot.Add(new LootInfo
					{
						ItemInfo = database.ItemInfos.Ensure(lootObj.EnsureId()),
						Rate = lootObj.EnsureInt("Rate")
					});
				}
			}
		}
	}
}
