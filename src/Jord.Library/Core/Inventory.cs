using System.Collections.Generic;
using System.Linq;
using Jord.Core.Items;

namespace Jord.Core
{
	public class Inventory
	{
		/// <summary>
		/// Gold
		/// </summary>
		public int Gold { get; set; }


		public List<ItemPile> Items { get; } = new List<ItemPile>();

		public int Count => Items.Count;

		public void Add(Item item, int quantity)
		{
			if (quantity == 0)
			{
				return;
			}

			ItemPile position = null;
			foreach (var p in Items)
			{
				if (item == p.Item)
				{
					position = p;
					break;
				}
			}

			if (position == null)
			{
				if (quantity < 0)
				{
					return;
				}

				Items.Add(new ItemPile
				{
					Item = item,
					Quantity = quantity
				});
			}
			else
			{
				position.Quantity += quantity;
				if (position.Quantity <= 0)
				{
					Items.Remove(position);
				}
			}
		}

		public bool Contains(Inventory other)
		{
			foreach (var otherItem in other.Items)
			{
				var found = false;
				foreach (var item in Items)
				{
					if (otherItem.Item == item.Item && otherItem.Quantity <= item.Quantity)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					return false;
				}
			}

			return true;
		}

		public Inventory Clone()
		{
			var _inventory = new Inventory();
			foreach (var itemPile in Items)
			{
				_inventory.Items.Add(itemPile.Clone());
			}

			return _inventory;
		}

		public override string ToString()
		{
			return string.Join(", ", from s in Items select s.ToString());
		}
	}
}
