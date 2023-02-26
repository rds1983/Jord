using System.Collections.Generic;

namespace Jord.Core.Abilities
{
	public class Effect: BaseObject
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public Dictionary<BonusType, int> Bonuses { get; } = new Dictionary<BonusType, int>();
	}
}
