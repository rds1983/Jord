using Jord.Core;
using Jord.Core.Items;

namespace Jord.Compiling.Loaders
{
	class ItemLoader : Loader<BaseItemInfo>
	{
		public ItemLoader() : base("ItemInfos")
		{
		}

		public override BaseItemInfo LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;
			var type = dataObj.OptionalString("Type");

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
					Capacity = dataObj.EnsureInt("Capacity")
				};

				result = waterContainer;
			}
			else if (type == "Weapon")
			{
				var weapon = new WeaponInfo
				{
					MinDamage = dataObj.EnsureInt("MinDamage"),
					MaxDamage = dataObj.EnsureInt("MaxDamage"),
					AttackType = dataObj.EnsureEnum<AttackType>("AttackType")
				};

				result = weapon;
			}
			else if (type == "Armor")
			{
				var armor = new ArmorInfo
				{
					ArmorClass = dataObj.EnsureInt("ArmorClass"),
					SubType = dataObj.EnsureEnum<EquipType>("SubType")
				};

				result = armor;
			}
			else
			{
				// Misc
				result = new BaseItemInfo();
			}

			module.EnsureBaseMapObject(dataObj, result, "Item" + id);
			result.Name = dataObj.EnsureString(Compiler.NameName);
			result.Price = dataObj.EnsureInt("Price");

			var tanningObj = dataObj.OptionalJObject("Tanning");
			if (tanningObj != null)
			{
				result.TanningResult = new Inventory();
				foreach(var pair in tanningObj)
				{
					var componentInfo = module.ItemInfos.Ensure(pair.Key);
					var quantity = pair.Value.ToInt();

					result.TanningResult.Add(new Item(componentInfo), quantity);
				}
			}

			string typeName;
			if (data.Properties != null && data.Properties.TryGetValue("Type", out typeName))
			{
				result.Type = typeName.ToEnum<ItemType>();
			}

			return result;
		}
	}
}