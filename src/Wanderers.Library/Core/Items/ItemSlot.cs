namespace Wanderers.Core.Items
{
	public class ItemSlot
	{
		public EquipType Slot
		{
			get; private set;
		}

		public Item Item
		{
			get; set;
		}

		public ItemSlot(EquipType slot)
		{
			Slot = slot;
		}
	}
}
