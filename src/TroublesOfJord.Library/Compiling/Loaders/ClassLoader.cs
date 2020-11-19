using System;
using TroublesOfJord.Core;
using TroublesOfJord.Core.Items;

namespace TroublesOfJord.Compiling.Loaders
{
	class ClassLoader: Loader<Class>
	{
		public ClassLoader(): base("Classes")
		{
		}

		public override Class LoadItem(Module module, string id, ObjectData data)
		{
			var result = new Class
			{
				Name = EnsureString(data, Compiler.NameName),
				Gold = EnsureInt(data, "Gold")
			};

			var equipmentData = EnsureJObject(data, "Equipment");
			foreach(var pair in equipmentData)
			{
				var slot = (EquipType)Enum.Parse(typeof(EquipType), pair.Key);
				var itemInfo = module.EnsureItemInfo(pair.Value.ToString());

				var item = new Item(itemInfo);

				result.Equipment.Equip(item);
			}

			return result;
		}
	}
}
