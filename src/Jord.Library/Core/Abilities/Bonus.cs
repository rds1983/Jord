﻿using System.Collections.Generic;

namespace Jord.Core.Abilities
{
	public enum BonusType
	{
		Attacks,
		HitRoll,
		OneHandedHitRoll,
		TwoHandedHitRoll
	}

	public class Bonus : AbilityEffect
	{
		public Dictionary<BonusType, int> Bonuses { get; } = new Dictionary<BonusType, int>();
	}
}