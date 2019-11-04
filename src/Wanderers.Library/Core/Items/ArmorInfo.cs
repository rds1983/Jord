namespace Wanderers.Core.Items
{
	public class ArmorInfo : EquipInfo
	{
		public override string BuildDescription()
		{
			return base.BuildDescription() + ", armor class: " + ArmorClass;
		}
	}
}