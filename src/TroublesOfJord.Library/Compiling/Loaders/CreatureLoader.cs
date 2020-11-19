using Newtonsoft.Json.Linq;
using TroublesOfJord.Core;
using TroublesOfJord.Core.Items;

namespace TroublesOfJord.Compiling.Loaders
{
	class CreatureLoader: Loader<CreatureInfo>
	{
		public CreatureLoader(): base("CreatureInfos")
		{
		}

		public override CreatureInfo LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;
			var result = new CreatureInfo
			{
				Name = dataObj.EnsureString(Compiler.NameName),
				Image = module.EnsureAppearance(dataObj, Compiler.ImageName),
				CreatureType = dataObj.EnsureEnum<CreatureType>("Type")
			};

			if (result.CreatureType != CreatureType.Instructor)
			{
				result.Gold = dataObj.EnsureInt("Gold");
			}

			if (result.CreatureType == CreatureType.Enemy)
			{
				result.Experience = dataObj.EnsureInt("Experience");
			}

			module.EnsureBaseMapObject(dataObj, result);

			if (result.CreatureType == CreatureType.Enemy)
			{
				result.ArmorClass = dataObj.EnsureInt("ArmorClass");
				result.HitRoll = dataObj.EnsureInt("HitRoll");
				result.MaxHp = dataObj.EnsureInt("MaxHp");
				result.HpRegen = dataObj.OptionalInt("HpRegen", Constants.DefaultHpRegen);
				result.MaxMana = dataObj.OptionalInt("MaxMana");
				result.MaxStamina = dataObj.OptionalInt("MaxStamina");

				var attacks = (JArray)dataObj["Attacks"];
				foreach(JObject attackObj in attacks)
				{
					result.Attacks.Add(attackObj.ParseAttack());
				}
			}

			JToken t;
			if (dataObj.TryGetValue("Inventory", out t))
			{
				JObject obj = (JObject)t;

				var inventory = new Inventory();

				foreach (var pair in obj)
				{
					inventory.Items.Add(new ItemPile
					{
						Item = new Item(module.ItemInfos.Ensure(pair.Key)),
						Quantity = (int)pair.Value
					});
				}

				result.Inventory = inventory;
			}

			return result;
		}
	}
}
