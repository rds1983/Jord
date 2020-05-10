using System;

namespace TroublesOfJord.Core.Items
{
	public class Item
	{
		private readonly BaseItemInfo _info;

		public BaseItemInfo Info
		{
			get
			{
				return _info;
			}
		}

		public Item(BaseItemInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			_info = info;
		}

		public string BuildDescription()
		{
			return _info.BuildDescription();
		}
	}
}
