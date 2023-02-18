using System;
using System.Diagnostics;
using Jord.Utils;

namespace Jord.Core
{
	partial class Creature
	{
		private const int AttackDurationInMs = 100;

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

					if (target.Stats.Life.CurrentHP <= 0 && target != TJ.Player)
					{
						OnKilledTarget(target);
						break;
					}
				}
			}

			var oldPosition = Position.ToVector2();
			var halfPosition = oldPosition + (target.DisplayPosition - oldPosition) * 0.5f;

			void onUpdate(float part)
			{
				if (part < 0.5f)
				{
					// Movement towards target
					DisplayPosition = oldPosition + (target.DisplayPosition - oldPosition) * part;
				}
				else
				{
					// Movement back
					DisplayPosition = halfPosition + (oldPosition - halfPosition) * part;
				}
			}

			TJ.ActivityService.AddOrderedActivity(onUpdate, () => DisplayPosition = oldPosition, AttackDurationInMs);
		}
	}
}
