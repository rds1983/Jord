using System.Collections.Generic;
using Jord.Core.Items;

namespace Jord.Core
{
	public class Inventory
	{
		private readonly List<ItemPile> _items = new List<ItemPile>();

		public List<ItemPile> Items
		{
			get
			{
				return _items;
			}
		}

		public void Add(Item item, int quantity)
		{
			if (quantity == 0)
			{
				return;
			}

			ItemPile position = null;
			foreach (var p in _items)
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

				_items.Add(new ItemPile
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
					_items.Remove(position);
				}
			}
		}
	}
}
