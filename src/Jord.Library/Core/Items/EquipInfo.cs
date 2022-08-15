using Jord.Serialization;

namespace Jord.Core.Items
{
	public abstract class EquipInfo: BaseItemInfo
	{
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
