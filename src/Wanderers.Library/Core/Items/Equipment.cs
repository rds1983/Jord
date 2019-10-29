using System;
using System.Linq;

namespace Wanderers.Core.Items
{
	public class Equipment
	{
		private readonly ItemSlot[] _items = new[]
		{
			new ItemSlot(EquipType.Light),
			new ItemSlot(EquipType.Body),
			new ItemSlot(EquipType.Head),
			new ItemSlot(EquipType.Legs),
			new ItemSlot(EquipType.Feet),
			new ItemSlot(EquipType.Hands),
			new ItemSlot(EquipType.Arms),
			new ItemSlot(EquipType.About),
			new ItemSlot(EquipType.Waist),
			new ItemSlot(EquipType.Shield),
			new ItemSlot(EquipType.Held),
			new ItemSlot(EquipType.Weapon)
		};

		public ItemSlot[] Items
		{
			get
			{
				return _items;
			}
		}

		public event EventHandler Changed;

		public Item GetItemByType(EquipType type)
		{
			return (from i in _items where i.Slot == type select i.Item).FirstOrDefault();
		}

		public void Equip(Item item)
		{
			var asEquip = item.Info as EquipInfo;
			if (asEquip == null)
			{
				throw new Exception("Only equipment can be worn");
			}

			var slots = (from i in _items where i.Slot == asEquip.SubType select i).ToArray();
			if (slots.Length == 0)
			{
				throw new Exception(string.Format("Slot {0} doesnt exist in Equipment", asEquip.SubType.ToString()));
			}

			int? index = null;

			// Try to find empty slot
			for (var i = 0; i < slots.Length; ++i)
			{
				if (slots[i].Item == null)
				{
					index = i;
					break;
				}
			}

			if (index == null)
			{
				return;
			}

			var slot = slots[index.Value];

			var result = slot.Item;

			slot.Item = item;

			Changed?.Invoke(this, EventArgs.Empty);
		}

		public Item Remove(int slotIndex)
		{
			var slot = _items[slotIndex];
			var result = slot.Item;
			slot.Item = null;

			Changed?.Invoke(this, EventArgs.Empty);

			return result;
		}
	}
}
