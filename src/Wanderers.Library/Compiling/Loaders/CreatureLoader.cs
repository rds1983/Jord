using Newtonsoft.Json.Linq;
using Wanderers.Core;
using Wanderers.Core.Items;

namespace Wanderers.Compiling
{
	public class CreatureLoader: Loader<CreatureInfo>
	{
		protected override ItemWithId LoadObject(CompilerContext context, string id, ObjectData data)
		{
			var creature = (CreatureInfo)base.LoadObject(context, id, data);

			JToken t;
			if (data.Object.TryGetValue("inventory", out t))
			{
				JObject obj = (JObject)t;

				var inventory = new Inventory();

				foreach (var pair in obj)
				{
					inventory.Items.Add(new ItemPile
					{
						Item = new Item(context.Module.ItemInfos[pair.Key]),
						Quantity = (int)pair.Value
					});
				}

				creature.Inventory = inventory;
			}

			return creature;
		}
	}
}
