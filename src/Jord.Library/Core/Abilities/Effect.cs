using System.Collections.Generic;

namespace Jord.Core.Abilities
{
	public enum EffectConditionVariable
	{
		IsWeaponOneHanded
	}

	public enum EffectConditionOperator
	{
		Equals
	}

	public class EffectCondition
	{
		public EffectConditionVariable Variable { get; }
		public EffectConditionOperator Operator { get; }
		public object Value { get; }

		public EffectCondition(EffectConditionVariable variable, EffectConditionOperator op, object value)
		{
			Variable = variable;
			Operator = op;
			Value = value;
		}
	}

	public class Effect: BaseObject
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public Dictionary<BonusType, int> Bonuses { get; } = new Dictionary<BonusType, int>();
		public List<EffectCondition> Conditions { get; } = new List<EffectCondition>();

	}
}
