using Jord.Core.Items;

namespace Jord.Core
{
	public class Class : BaseObject
	{
		public string Name
		{
			get; set;
		}

		public int Gold;

		public readonly Equipment Equipment = new Equipment();

		public int HpMultiplier;
		public int ManaMultiplier;
		public int StaminaMultiplier;
		public float HpRegenMultiplier;
		public float ManaRegenMultiplier;
		public float StaminaRegenMultiplier;
	}
}