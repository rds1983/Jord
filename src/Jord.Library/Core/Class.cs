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
		public int Melee { get; set; }
		public int Armor { get; set; }
		public int Evasion { get; set; }
		public int Blocking { get; set; }
		public float MeleePerLevel { get; set; }
		public float EvasionPerLevel { get; set; }
		public float BlockingPerLevel { get; set; }

		public int HpMultiplier { get; set; }
		public int ManaMultiplier { get; set; }
		public int StaminaMultiplier { get; set; }
		public float HpRegenMultiplier { get; set; }
		public float ManaRegenMultiplier { get; set; }
		public float StaminaRegenMultiplier { get; set; }
	}
}