using TroublesOfJord.Core;
using TroublesOfJord.Core.Items;

namespace TroublesOfJord.Compiling.Loaders
{
	class ItemLoader : Loader<BaseItemInfo>
	{
		public ItemLoader() : base("ItemInfos")
		{
		}

		public override BaseItemInfo LoadItem(Module module, string id, ObjectData data)
		{
			BaseItemInfo result = null;

			var type = EnsureString(data, "Type");
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
					Capacity = EnsureInt(data, "Capacity")
				};

				result = waterContainer;
			}
			else if (type == "Weapon")
			{
				var weapon = new WeaponInfo
				{
					MinDamage = EnsureInt(data, "MinDamage"),
					MaxDamage = EnsureInt(data, "MaxDamage"),
					AttackType = EnsureEnum<AttackType>(data, "AttackType")
				};

				result = weapon;
			}
			else if (type == "Armor")
			{
				var armor = new ArmorInfo
				{
					ArmorClass = EnsureInt(data, "ArmorClass")
				};

				result = armor;
			}

			result.Name = EnsureString(data, Compiler.NameName);
			result.Price = EnsureInt(data, "Price");

			return result;
		}
	}
}