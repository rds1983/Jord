using Newtonsoft.Json.Linq;
using Jord.Core;
using Jord.Core.Items;

namespace Jord.Compiling.Loaders
{
	class CreatureLoader: Loader<CreatureInfo>
	{
		private const string DungeonFilterName = "DungeonFilter";

		public CreatureLoader(): base("CreatureInfos")
		{
		}

		public override CreatureInfo LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;
			var result = new CreatureInfo
			{
				Name = dataObj.EnsureString(Compiler.NameName),
				CreatureType = dataObj.EnsureEnum<CreatureType>("Type"),
				MinimumLevel = dataObj.OptionalNullableInt("MinimumLevel")
			};

			string dungeonFilter;
			if (data.Properties != null && data.Properties.TryGetValue(DungeonFilterName, out dungeonFilter))
			{
				result.DungeonFilter = dungeonFilter;
			}

			if (result.CreatureType != CreatureType.Instructor)
			{
				result.Gold = dataObj.OptionalInt("Gold");
			}

			if (result.CreatureType == CreatureType.Enemy)
			{
				result.Experience = dataObj.EnsureInt("Experience");
			}

			module.EnsureBaseMapObject(dataObj, result, "Creature" + id);

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
