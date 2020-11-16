using System.Diagnostics;
using TroublesOfJord.Utils;

namespace TroublesOfJord.Core
{
	partial class Creature
	{
		protected virtual void OnKilledTarget(Creature target)
		{
		}

		public void Attack(Creature target)
		{
			var battleStats = Stats.Battle;

			var attacks = battleStats.Attacks;
			if (attacks == null || attacks.Length == 0)
			{
				return;
			}

			foreach (var attack in attacks)
			{
				var armorClass = target.Stats.Battle.ArmorClass;
				var attackRoll = 50 + battleStats.HitRoll * 8 - armorClass;
				Debug.WriteLine("{0} against {1}'s attack roll is {2}", Name, target.Name, attackRoll);
				var damage = MathUtils.Random.Next(attack.MinDamage, attack.MaxDamage + 1);
				if (!MathUtils.RollPercentage(attackRoll) || damage <= 0)
				{
					// Miss
					var message = Strings.GetMissMessage(Name, target.Name, attack.AttackType);
					TJ.GameLog(message);
				}
				else
				{
					target.Stats.Life.CurrentHP -= damage;
					var message = Strings.GetAttackMessage(damage, Name, target.Name, attack.AttackType);
					TJ.GameLog(message);

					if (target.Stats.Life.CurrentHP <= 0)
					{
						OnKilledTarget(target);
						break;
					}
				}
			}
		}
	}
}
