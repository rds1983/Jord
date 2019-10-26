﻿using System.Collections.Generic;
using Wanderers.Compiling;

namespace Wanderers.Core
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

		[OptionalField]
		public Inventory Inventory { get; set; }

		[OptionalField]
		public List<AttackInfo> Attacks => _attacks;

		public CreatureInfo()
		{
			Inventory = new Inventory();
		}
	}
}