using TroublesOfJord.Utils;

namespace TroublesOfJord.Core
{
	partial class Creature
	{
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
				var attackRoll = MathUtils.RollD20() + battleStats.HitRoll;
				var damage = MathUtils.Random.Next(attack.MinDamage, attack.MaxDamage + 1);

				if (attackRoll < target.Stats.Battle.ArmorClass || damage <= 0)
				{
					// Miss
					var message = AttackInfo.GetMissMessage(attackRoll, Name, target.Name, attack.AttackType);
					TJ.GameLog(message);
				}
				else
				{
					target.Stats.Life.CurrentHP -= damage;

					if (target.Stats.Life.CurrentHP > 0)
					{
						var message = AttackInfo.GetAttackMessage(attackRoll, damage, Name, target.Name, attack.AttackType);
						TJ.GameLog(message);
					}
					else if (target is NonPlayer)
					{
						var message = AttackInfo.GetNpcDeathMessage(target.Name);
						TJ.GameLog(message);

						// Death
						target.Remove();
						break;
					}
				}
			}
		}
	}
}
