using System.Collections.Generic;
using TroublesOfJord.Core.Items;

namespace TroublesOfJord.Core.Abilities
{
	public enum BonusType
	{
		Attacks
	}

	public class AbilityInfo: BaseObject
	{
		public string Name;
		public AbilityRequirement[] Requirements;
		public BaseItemInfo Manual;

		public Dictionary<BonusType, int> Bonuses { get; } = new Dictionary<BonusType, int>();
	}
}
