using Wanderers.Compiling;

namespace Wanderers.Core
{
	public class CreatureInfo : ItemWithId
	{
		public string Name
		{
			get; set;
		}

		public Appearance Image
		{
			get; set;
		}

		[OptionalField]
		public bool IsMerchant
		{
			get; set;
		}

		public int Gold
		{
			get; set;
		}

		public Inventory Inventory
		{
			get; set;
		}

		public CreatureInfo()
		{
			Inventory = new Inventory();
		}
	}
}