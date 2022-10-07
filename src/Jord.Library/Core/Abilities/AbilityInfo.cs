using System.Collections.Generic;

namespace Jord.Core.Abilities
{
	public enum AbilityType
	{
		Permanent,
		Instant
	}

	public class AbilityInfo: BaseObject
	{
		public string Name { get; set; }
		public int Mana { get; set; }
		public AbilityType Type { get; set; }
		public string Description { get; set; }

		public AbilityRequirement[] Requirements { get; set; }
		public AbilityEffect[] Effects { get; set; }

		public Dictionary<BonusType, int> Bonuses { get; } = new Dictionary<BonusType, int>();
	}
}
