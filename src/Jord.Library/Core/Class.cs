using Jord.Core.Abilities;
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
		public int MeleeMastery { get; set; }
		public int ArmorClass { get; set; }
		public int EvasionRating { get; set; }
		public int BlockingRating { get; set; }

		public int HpMultiplier { get; set; }
		public int ManaMultiplier { get; set; }
		public int StaminaMultiplier { get; set; }
		public float HpRegenMultiplier { get; set; }
		public float ManaRegenMultiplier { get; set; }
		public float StaminaRegenMultiplier { get; set; }
	}
}