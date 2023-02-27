using System;
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

		public ItemSlot[] Items
		{
			get
			{
				return _items;
			}
		}

		public event EventHandler Changed;

		public Item GetItemByType(EquipType type) => Items[(int)type].Item;

		public void Equip(Item item)
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

			Remove(index.Value);
			var slot = Items[index.Value];
			slot.Item = item;

			Changed?.Invoke(this, EventArgs.Empty);
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
