using System;
using System.Collections.Generic;
using System.Linq;

namespace Jord.Core.Items
{
	public enum EquipType
	{
		Light,
		Body,
		Head,
		Legs,
		Feet,
		Hands,
		Arms,
		Waist,
		LeftHand,
		RightHand,
	}

	public class ItemSlot
	{
		public EquipType Slot { get; private set; }

		public Item Item { get; set; }

		public ItemSlot(EquipType slot)
		{
			Slot = slot;
		}
	}


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
			new ItemSlot(EquipType.Waist),
			new ItemSlot(EquipType.LeftHand),
			new ItemSlot(EquipType.RightHand)
		};

		public ItemSlot[] Items => _items;

		public event EventHandler Changed;

		public Item GetItemByType(EquipType type) => Items[(int)type].Item;

		public Item[] Equip(Item item)
		{
			var asEquip = item.Info as EquipInfo;
			if (asEquip == null)
			{
				throw new Exception("Only equipment can be worn");
			}

			int? index = null;
			var asBasicArmorInfo = asEquip as BasicArmorInfo;
			if (asBasicArmorInfo != null)
			{
				index = (int)asBasicArmorInfo.SubType;
			}

			var asShieldInfo = asEquip as ShieldInfo;
			if (asShieldInfo != null)
			{
				index = (int)EquipType.LeftHand;
			}

			var asWeaponInfo = asEquip as WeaponInfo;
			if (asWeaponInfo != null)
			{
				index = (int)EquipType.RightHand;
			}

			if (index == null)
			{
				throw new Exception($"Couldn't find suitable slot for item {asEquip}");
			}

			var slot = Items[index.Value];

			var removedItems = new List<Item>();
			if (slot.Item != null)
			{
				removedItems.Add(slot.Item);
			}

			slot.Item = item;

			Changed?.Invoke(this, EventArgs.Empty);

			return removedItems.ToArray();
		}

		public Item Remove(int slotIndex)
		{
			var slot = _items[slotIndex];
			if (slot.Item == null)
			{
				return null;
			}

			var result = slot.Item;
			slot.Item = null;

			Changed?.Invoke(this, EventArgs.Empty);

			return result;
		}
	}
}
