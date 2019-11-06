using System;

namespace Wanderers.Core
{
	public class CreatureStats
	{
		public class LifeStats
		{
			private int _currentHp, _maximumHp;
			private int _currentMana, _maximumMana;
			private int _currentStamina, _maximumStamina;

			public int CurrentHP
			{
				get
				{
					return _currentHp;
				}

				set
				{
					if (_currentHp == value)
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

			public int CurrentMana
			{
				get
				{
					return _currentMana;
				}

				set
				{
					if (_currentMana == value)
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

			public int CurrentStamina
			{
				get
				{
					return _currentStamina;
				}

				set
				{
					if (_currentStamina == value)
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
			public int ArmorClass;
			public int HitRoll;
			public AttackInfo[] Attacks;
		}

		public LifeStats Life { get; } = new LifeStats();
		public BattleStats Battle { get; } = new BattleStats();
	}
}
