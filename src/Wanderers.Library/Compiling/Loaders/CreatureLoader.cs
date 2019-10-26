using Newtonsoft.Json.Linq;
using Wanderers.Core;
using Wanderers.Core.Items;

namespace Wanderers.Compiling.Loaders
{
	public class CreatureLoader: Loader<CreatureInfo>
	{
		public CreatureLoader(): base("CreatureInfos")
		{
		}

		public override BaseObject LoadItem(CompilerContext context, string id, ObjectData data)
		{
			var creature = (CreatureInfo)base.LoadItem(context, id, data);

			JToken t;
			if (data.Data.TryGetValue("Inventory", out t))
			{
				JObject obj = (JObject)t;

				var inventory = new Inventory();

				foreach (var pair in obj)
				{
					inventory.Items.Add(new ItemPile
					{
						Item = new Item(context.Module.EnsureItemInfo(pair.Key)),
						Quantity = (int)pair.Value
					});
				}

				creature.Inventory = inventory;
			}

			return creature;
		}
	}
}
