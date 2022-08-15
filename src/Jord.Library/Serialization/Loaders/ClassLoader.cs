using Newtonsoft.Json.Linq;
using System;
using Jord.Core;
using Jord.Core.Items;

namespace Jord.Serialization.Loaders
{
	class ClassLoader: Loader<Class>
	{
		public ClassLoader(): base("Classes")
		{
		}

		public override Class LoadItem(Module module, string id, ObjectData data)
		{
			var dataObj = data.Data;
			var result = new Class
			{
				Name = dataObj.EnsureString(Serializer.NameName),
				Gold = dataObj.EnsureInt("Gold"),
				HpMultiplier = dataObj.EnsureInt("HpMultiplier"),
				ManaMultiplier = dataObj.EnsureInt("ManaMultiplier"),
				StaminaMultiplier = dataObj.EnsureInt("StaminaMultiplier"),
				HpRegenMultiplier = dataObj.EnsureFloat("HpRegenMultiplier"),
				ManaRegenMultiplier = dataObj.EnsureFloat("ManaRegenMultiplier"),
				StaminaRegenMultiplier = dataObj.EnsureFloat("StaminaRegenMultiplier"),
			};

			var equipmentData = dataObj.EnsureJObject("Equipment");
			foreach(var pair in equipmentData)
			{
				var slot = (EquipType)Enum.Parse(typeof(EquipType), pair.Key);
				var itemInfo = module.ItemInfos.Ensure(pair.Value.ToString());

				var item = new Item(itemInfo);

				result.Equipment.Equip(item);
			}

			return result;
		}
	}
}
