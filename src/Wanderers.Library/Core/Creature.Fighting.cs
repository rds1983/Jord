using Microsoft.Xna.Framework;
using System;
using Wanderers.Utils;

namespace Wanderers.Core
{
	partial class Creature
	{
		private const int TargetAttackDelayInMs = 1000;
		private const int AttackMoveInMs = 200;
		private const int TwoAttackMoveInMs = AttackMoveInMs * 2;

		private const int DyingOpaquePeriodInMs = 1000;
		private const int DyingTransparentPeriodInMs = 1000;

		private DateTime? _attackStart;
		private int _currentAttackIndex;

		private void ProcessFighting()
		{
			if (!IsAttackable(AttackTarget))
			{
				// End fighting
				State = CreatureState.Idle;
				return;
			}

			var battleStats = Stats.Battle;
			var attacks = battleStats.Attacks;
			if (attacks == null || attacks.Length == 0)
			{
				return;
			}

			var endFighting = false;
			var now = DateTime.Now;
			var span = now - _actionStart;
			if (span.TotalMilliseconds >= AttackDelayInMs)
			{
				_attackStart = now;
				_actionStart = now;

				if (_currentAttackIndex >= attacks.Length)
				{
					_currentAttackIndex = 0;
				}

				var attack = attacks[_currentAttackIndex];

				var attackRoll = MathUtils.RollD20() + battleStats.HitRoll;
				var damage = MathUtils.Random.Next(attack.MinDamage, attack.MaxDamage + 1);

				if (attackRoll < AttackTarget.Stats.Battle.ArmorClass || damage <= 0)
				{
					// Miss
					var message = AttackInfo.GetMissMessage(attackRoll, Name, AttackTarget.Name, attack.AttackType);
					TJ.GameLog(message);
				}
				else
				{
					AttackTarget.Stats.Life.CurrentHP -= damage;

					if (AttackTarget.Stats.Life.CurrentHP > 0)
					{
						var message = AttackInfo.GetAttackMessage(attackRoll, damage, Name, AttackTarget.Name, attack.AttackType);
						TJ.GameLog(message);
					} else if (AttackTarget is NonPlayer)
					{
						var message = AttackInfo.GetNpcDeathMessage(AttackTarget.Name);
						TJ.GameLog(message);

						// Death
						AttackTarget.State = CreatureState.Dying;

						// End fighting
						endFighting = true;
					}
				}

				++_currentAttackIndex;

				AttackDelayInMs = attack.Delay;
			}

			if (_attackStart != null)
			{
				span = now - _attackStart.Value;
				var delta = new Vector2(AttackTarget.Position.X - Position.X, AttackTarget.Position.Y - Position.Y) / 2;

				if (span.TotalMilliseconds < AttackMoveInMs)
				{
					// First phase
					DisplayPosition = new Vector2(
						Position.X + delta.X * (float)span.TotalMilliseconds / AttackMoveInMs,
						Position.Y + delta.Y * (float)span.TotalMilliseconds / AttackMoveInMs);
				}
				else if (span.TotalMilliseconds < TwoAttackMoveInMs)
				{
					// Second phase
					var f = (TwoAttackMoveInMs - (float)span.TotalMilliseconds);
					DisplayPosition = new Vector2(
						Position.X + delta.X * f / TwoAttackMoveInMs,
						Position.Y + delta.Y * f / TwoAttackMoveInMs);
				}
				else
				{
					_attackStart = null;
				}
			}

			if (endFighting)
			{
				State = CreatureState.Idle;
			}
		}

		private bool IsAttackable(Creature target)
		{
			if (Math.Abs(Position.X - target.Position.X) > 1 ||
				Math.Abs(Position.Y - target.Position.Y) > 1)
			{
				// Too far away
				return false;
			}

			return true;
		}

		public bool Attack(Creature target)
		{
			if (!IsAttackable(target))
			{
				return false;
			}

			var now = DateTime.Now;

			State = CreatureState.Fighting;
			AttackTarget = target;
			AttackDelayInMs = 0;
			_currentAttackIndex = 0;

			target.AttackTarget = this;
			target.State = CreatureState.Fighting;
			target.AttackDelayInMs = TargetAttackDelayInMs;
			target._currentAttackIndex = 0;

			TJ.GameLog("{0} attacked {1}...", Name, target.Name);

			return true;
		}

		private void ProcessDying()
		{
			var passed = (DateTime.Now - _actionStart).TotalMilliseconds;
			if (passed < DyingOpaquePeriodInMs)
			{
				// Do nothing
				return;
			}

			passed -= DyingOpaquePeriodInMs;
			if (passed < DyingTransparentPeriodInMs)
			{
				// Update opacity
				Opacity = (float)(DyingTransparentPeriodInMs - passed) / DyingTransparentPeriodInMs;
				return;
			}

			// Remove
			Remove();
		}
	}
}
