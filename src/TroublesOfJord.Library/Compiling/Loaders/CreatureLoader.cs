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
			var result = new CreatureInfo
			{
				Name = EnsureString(data, Compiler.NameName),
				Image = EnsureAppearance(module, data, Compiler.ImageName),
				CreatureType = EnsureEnum<CreatureType>(data.Data, data.Source, "Type")
			};

			if (result.CreatureType != CreatureType.Instructor)
			{
				result.Gold = EnsureInt(data, "Gold");
			}

			if (result.CreatureType == CreatureType.Enemy)
			{
				result.Experience = EnsureInt(data, "Experience");
			}

			EnsureBaseMapObject(module, data, result);

			if (result.CreatureType == CreatureType.Enemy)
			{
				result.ArmorClass = EnsureInt(data, "ArmorClass");
				result.HitRoll = EnsureInt(data, "HitRoll");
				result.MaxHp = EnsureInt(data, "MaxHp");
				result.HpRegen = OptionalInt(data, "HpRegen", Constants.DefaultHpRegen);
				result.MaxMana = OptionalInt(data, "MaxMana");
				result.MaxStamina = OptionalInt(data, "MaxStamina");

				var attacks = (JArray)data.Data["Attacks"];
				foreach(JObject attackObj in attacks)
				{
					result.Attacks.Add(ParseAttack(attackObj, data.Source));
				}
			}

			JToken t;
			if (data.Data.TryGetValue("Inventory", out t))
			{
				JObject obj = (JObject)t;

				var inventory = new Inventory();

				foreach (var pair in obj)
				{
					inventory.Items.Add(new ItemPile
					{
						Item = new Item(module.EnsureItemInfo(pair.Key)),
						Quantity = (int)pair.Value
					});
				}

				result.Inventory = inventory;
			}

			return result;
		}
	}
}
