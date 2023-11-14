using System;
using System.Collections.Generic;
using Jord.Core;
using Jord.Core.Items;
using Newtonsoft.Json.Linq;

namespace Jord.Loading
{
	class ClassLoader : BaseObjectLoader<Class>
	{
		public static readonly ClassLoader Instance = new ClassLoader();

		protected override Class CreateObject(string source, JObject data, Dictionary<string, string> properties, out Action<Database> secondRunAction)
		{
			var result = new Class
			{
				Name = data.EnsureString("Name"),
				Gold = data.EnsureInt("Gold"),
				HpMultiplier = data.EnsureInt("HpMultiplier"),
				ManaMultiplier = data.EnsureInt("ManaMultiplier"),
				StaminaMultiplier = data.EnsureInt("StaminaMultiplier"),
				HpRegenMultiplier = data.EnsureFloat("HpRegenMultiplier"),
				ManaRegenMultiplier = data.EnsureFloat("ManaRegenMultiplier"),
				StaminaRegenMultiplier = data.EnsureFloat("StaminaRegenMultiplier"),
				MeleeMastery = data.OptionalInt("MeleeMastery"),
				ArmorRating = data.OptionalInt("ArmorRating"),
				EvasionRating = data.OptionalInt("EvasionRating"),
				BlockingRating = data.OptionalInt("BlockingRating"),
				MeleeMasteryPerLevel = data.EnsureFloat("MeleeMasteryPerLevel"),
				EvasionRatingPerLevel = data.EnsureFloat("EvasionRatingPerLevel"),
				BlockingRatingPerLevel = data.EnsureFloat("BlockingRatingPerLevel")
			};

			secondRunAction = db => SecondRun(result, data, db);

			return result;
		}

		private void SecondRun(Class result, JObject data, Database database)
		{
			var equipmentData = data.EnsureJObject("Equipment");
			foreach (var pair in equipmentData)
			{
				var slot = (EquipType)Enum.Parse(typeof(EquipType), pair.Key);
				var itemInfo = database.ItemInfos.Ensure(pair.Value.ToString());

				var item = new Item(itemInfo);

				result.Equipment.Equip(item);
			}
		}
	}
}
