﻿using Microsoft.Xna.Framework;

namespace Jord.Core.Abilities
{
	public class Perk : BaseObject
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public int Tier { get; set; }
		public Point Position { get; set; }

		public Effect[] AddsEffects { get; set; }
		public Perk[] RequiresPerks { get; set; }

		public static int GetMinimumLevelForTier(int tier)
		{
			if (tier <= 1)
			{
				return 1;
			}

			return (tier - 1) * 10;
		}
	}
}
