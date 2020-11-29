using Newtonsoft.Json.Linq;
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

			BaseItemInfo result = null;

			var type = dataObj.EnsureString("Type");
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

			result.Name = dataObj.EnsureString(Compiler.NameName);
			result.Price = dataObj.EnsureInt("Price");

			return result;
		}
	}
}