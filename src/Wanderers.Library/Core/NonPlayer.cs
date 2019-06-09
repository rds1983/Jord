using System;

namespace Wanderers.Core
{
	public class NonPlayer : Creature
	{
		private readonly CreatureInfo _info;

		public override Appearance Image
		{
			get
			{
				return _info.Image;
			}
		}

		public CreatureInfo Info
		{
			get
			{
				return _info;
			}
		}

		public NonPlayer(CreatureInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			_info = info;
			Name = _info.Name;
			Gold = _info.Gold;

			foreach (var itemPile in info.Inventory.Items)
			{
				Inventory.Items.Add(itemPile.Clone());
			}
		}
	}
}