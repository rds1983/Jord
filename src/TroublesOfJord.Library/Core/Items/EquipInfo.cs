using TroublesOfJord.Compiling;

namespace TroublesOfJord.Core.Items
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
