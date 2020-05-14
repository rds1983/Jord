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
				Gold = EnsureInt(data, "Gold"),
				IsMerchant = OptionalBool(data, "IsMerchant", false),
				IsAttackable = OptionalBool(data, "IsAttackable", false),
			};

			EnsureBaseMapObject(module, data, result);

			if (result.IsAttackable)
			{
				result.ArmorClass = EnsureInt(data, "ArmorClass");
				result.HitRoll = EnsureInt(data, "HitRoll");

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
