using Jord.Core.Items;

namespace Jord.Core
{
	public class Class : BaseObject
	{
		public string Name
		{
			get; set;
		}

		public int Gold { get; set; }

		public readonly Equipment Equipment = new Equipment();

		public int HpMultiplier { get; set; }
		public int ManaMultiplier { get; set; }
		public int StaminaMultiplier { get; set; }
		public float HpRegenMultiplier { get; set; }
		public float ManaRegenMultiplier { get; set; }
		public float StaminaRegenMultiplier { get; set; }
	}
}