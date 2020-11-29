namespace Jord.Core.Items
{
	public class WaterContainerInfo : BaseItemInfo
	{
		public int Capacity
		{
			get; set;
		}

		public override string BuildDescription()
		{
			return base.BuildDescription() + ", capacity: " + Capacity;
		}
	}
}
