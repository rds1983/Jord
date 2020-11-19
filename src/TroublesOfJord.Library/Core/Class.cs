using TroublesOfJord.Core.Items;

namespace TroublesOfJord.Core
{
	public class Class : BaseObject
	{
		public string Name
		{
			get; set;
		}

		public int Gold;

		public readonly Equipment Equipment = new Equipment();
	}
}