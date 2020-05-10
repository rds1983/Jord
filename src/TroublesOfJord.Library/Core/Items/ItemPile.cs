using System;

namespace TroublesOfJord.Core.Items
{
	public class ItemPile
	{
		private int _quantity = 0;

		public Item Item
		{
			get; set;
		}

		public int Quantity
		{
			get
			{
				return _quantity;
			}

			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				_quantity = value;
			}
		}

		public ItemPile Clone()
		{
			return new ItemPile
			{
				Item = Item,
				Quantity = Quantity
			};
		}

		public override string ToString()
		{
			if (Quantity == 1)
			{
				return Item.Info.Name;
			}

			return string.Format("{0} ({1})", Item.Info.Name, Quantity);
		}
	}
}
