﻿using System.Collections.Generic;
using TroublesOfJord.Compiling;

namespace TroublesOfJord.Core
{
	public class CreatureInfo : BaseObject
	{
		private readonly List<AttackInfo> _attacks = new List<AttackInfo>();

		public string Name { get; set; }
		public Appearance Image { get; set; }
		public int Gold { get; set; }

		[OptionalField]
		public bool IsMerchant { get; set; }

		[OptionalField]
		public bool IsAttackable { get; set; }

		[IgnoreField]
		public Inventory Inventory { get; set; }

		[OptionalField]
		public List<AttackInfo> Attacks => _attacks;

		[OptionalField]
		public int ArmorClass;

		[OptionalField]
		public int HitRoll;

		[OptionalField]
		public int MaxHp, MaxMana, MaxStamina;

		public CreatureInfo()
		{
			Inventory = new Inventory();
		}
	}
}