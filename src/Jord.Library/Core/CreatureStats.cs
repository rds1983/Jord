using System;
using Jord.Utils;

namespace Jord.Core
{
	public class CreatureStats
	{
		public class LifeStats
		{
			private int _maximumHp, _maximumMana, _maximumStamina;
			private float _currentHp, _currentMana, _currentStamina;
			private float _hpRegen, _manaRegen, _staminaRegen;

			public float CurrentHP
			{
				get
				{
					return _currentHp;
				}

				set
				{
					if (_currentHp.EpsilonEquals(value))
					{
						return;
					}

					_currentHp = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public int MaximumHP
			{
				get
				{
					return _maximumHp;
				}

				set
				{
					if (_maximumHp == value)
					{
						return;
					}

					_maximumHp = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public float HpRegen
			{
				get
				{
					return _hpRegen;
				}

				set
				{
					if (_hpRegen.EpsilonEquals(value))
					{
						return;
					}

					_hpRegen = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public float CurrentMana
			{
				get
				{
					return _currentMana;
				}

				set
				{
					if (_currentMana.EpsilonEquals(value))
					{
						return;
					}

					_currentMana = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public int MaximumMana
			{
				get
				{
					return _maximumMana;
				}

				set
				{
					if (_maximumMana == value)
					{
						return;
					}

					_maximumMana = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public float ManaRegen
			{
				get
				{
					return _manaRegen;
				}

				set
				{
					if (_manaRegen.EpsilonEquals(value))
					{
						return;
					}

					_manaRegen = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public float CurrentStamina
			{
				get
				{
					return _currentStamina;
				}

				set
				{
					if (_currentStamina.EpsilonEquals(value))
					{
						return;
					}

					_currentStamina = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public int MaximumStamina
			{
				get
				{
					return _maximumStamina;
				}

				set
				{
					if (_maximumStamina == value)
					{
						return;
					}

					_maximumStamina = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public float StaminaRegen
			{
				get
				{
					return _staminaRegen;
				}

				set
				{
					if (_staminaRegen.EpsilonEquals(value))
					{
						return;
					}

					_staminaRegen = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public event EventHandler Changed;

			public void Restore()
			{
				CurrentHP = MaximumHP;
				CurrentMana = MaximumMana;
				CurrentStamina = MaximumStamina;
			}
		}

		public class BattleStats
		{
			public int ArmorClass { get; set; }
			public int HitRoll { get; set; }
			public AttackInfo[] Attacks { get; set; }
		}

		public LifeStats Life { get; } = new LifeStats();
		public BattleStats Battle { get; } = new BattleStats();
	}
}
