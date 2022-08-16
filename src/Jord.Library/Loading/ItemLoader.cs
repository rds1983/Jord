﻿using Jord.Core;
using Jord.Core.Items;
using Newtonsoft.Json.Linq;
using System;

namespace Jord.Loading
{
	class ItemLoader : BaseMapObjectLoader<BaseItemInfo>
	{
		public static readonly ItemLoader Instance = new ItemLoader();

		protected override BaseItemInfo CreateObject(string source, JObject data, out Action<Database> secondRunAction)
		{
			var type = data.OptionalString("Type");

			BaseItemInfo result;
			if (type == "Food")
			{
				var food = new FoodInfo
				{

				};

				result = food;
			}
			else if (type == "WaterContainer")
			{
				var waterContainer = new WaterContainerInfo
				{
					Capacity = data.EnsureInt("Capacity")
				};

				result = waterContainer;
			}
			else if (type == "Weapon")
			{
				var weapon = new WeaponInfo
				{
					MinDamage = data.EnsureInt("MinDamage"),
					MaxDamage = data.EnsureInt("MaxDamage"),
					AttackType = data.EnsureEnum<AttackType>("AttackType")
				};

				result = weapon;
			}
			else if (type == "Armor")
			{
				var armor = new ArmorInfo
				{
					ArmorClass = data.EnsureInt("ArmorClass"),
					SubType = data.EnsureEnum<EquipType>("SubType")
				};

				result = armor;
			}
			else
			{
				// Misc
				result = new BaseItemInfo();
			}

			result.Name = data.EnsureString("Name");
			result.Price = data.EnsureInt("Price");

			/*			string typeName;
						if (data.Properties != null && data.Properties.TryGetValue("Type", out typeName))
						{
							result.Type = typeName.ToEnum<ItemType>();
						}*/

			secondRunAction = db => SecondRun(result, data, db);

			return result;
		}

		private void SecondRun(BaseItemInfo result, JObject data, Database database)
		{
			var tanningObj = data.OptionalJObject("Tanning");
			if (tanningObj != null)
			{
				result.Tanning = new Inventory();
				foreach (var pair in tanningObj)
				{
					var componentInfo = database.ItemInfos.Ensure(pair.Key);
					var quantity = pair.Value.ToInt();

					result.Tanning.Add(new Item(componentInfo), quantity);
				}
			}

			var craftingObj = data.OptionalJObject("Crafting");
			if (craftingObj != null)
			{
				result.Crafting = new Inventory();
				foreach (var pair in craftingObj)
				{
					var componentInfo = database.ItemInfos.Ensure(pair.Key);
					var quantity = pair.Value.ToInt();

					result.Crafting.Add(new Item(componentInfo), quantity);
				}
			}
		}
	}
}