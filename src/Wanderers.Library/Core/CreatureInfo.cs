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

		[OptionalField]
		public bool IsAttackable
		{
			get; set;
		}

		public int Gold
		{
			get; set;
		}

		[OptionalField]
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