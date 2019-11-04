using Wanderers.Compiling;

namespace Wanderers.Core.Items
{
	public abstract class EquipInfo: BaseItemInfo
	{
		[OptionalField]
		public int ArmorClass
		{
			get; set;
		}

		public EquipType SubType
		{
			get; set;
		}
	}
}
