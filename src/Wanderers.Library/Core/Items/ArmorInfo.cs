namespace Wanderers.Core.Items
{
	public class ArmorInfo : EquipInfo
	{
		public int ArmorClass
		{
			get; set;
		}

		public override string BuildDescription()
		{
			return base.BuildDescription() + ", armor class: " + ArmorClass;
		}
	}
}