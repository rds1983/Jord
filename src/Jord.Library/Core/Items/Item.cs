using System;

namespace Jord.Core.Items
{
	public class Item
	{
		public BaseItemInfo Info { get; }

		public Item(BaseItemInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			Info = info;
		}

		public string BuildDescription()
		{
			return Info.BuildDescription();
		}

		public override bool Equals(object obj)
		{
			var asItem = obj as Item;
			if (ReferenceEquals(asItem, null))
			{
				return false;
			}

			if (ReferenceEquals(this, asItem))
			{
				return true;
			}

			return Info == asItem.Info;
		}

		public static bool operator ==(Item a, Item b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(Item a, Item b)
		{
			return !(a == b);
		}
	}
}
