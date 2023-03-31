using System;
using System.Collections.Generic;
using System.Text;

namespace Jord.Core.Items
{
	public enum ItemType
	{
		Equipment,
		Component,
		Trash,
		Skin
	}

	public class BaseItemInfo : BaseMapObject
	{
		public string Name { get; set; }

		public int Price { get; set; }

		public Inventory Tanning { get; set; }

		public Inventory Crafting { get; set; }

		public ItemType Type { get; set; }

		public virtual string BuildDescription()
		{
			return Type.ToString().ToLower();
		}
	}
}
