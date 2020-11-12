using System.Collections.Generic;
using TroublesOfJord.Core.Items;

namespace TroublesOfJord.Core.Abilities
{
	public enum AbilityType
	{
		Permanent,
		Instant
	}

	public class AbilityInfo: BaseObject
	{
		public string Name;
		public int Mana;
		public AbilityType Type;
		public string Description;
		
		public AbilityRequirement[] Requirements;
		public AbilityEffect[] Effects;
		public BaseItemInfo Manual;

		public Dictionary<BonusType, int> Bonuses { get; } = new Dictionary<BonusType, int>();
	}
}
