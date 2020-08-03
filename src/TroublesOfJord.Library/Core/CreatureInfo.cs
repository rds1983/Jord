using System.Collections.Generic;

namespace TroublesOfJord.Core
{
	public enum CreatureType
	{
		Enemy,
		NeutralEnemy,
		Merchant,
		Instructor
	}

	public class CreatureInfo : BaseMapObject
	{
		private readonly List<AttackInfo> _attacks = new List<AttackInfo>();

		public string Name { get; set; }
		public int Gold { get; set; }

		public CreatureType CreatureType;

		public Inventory Inventory { get; set; }

		public List<AttackInfo> Attacks => _attacks;

		public int ArmorClass;

		public int HitRoll;

		public int MaxHp, MaxMana, MaxStamina;

		public CreatureInfo()
		{
			Inventory = new Inventory();
		}
	}
}