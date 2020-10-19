using System;

namespace TroublesOfJord.Core
{
	public class CreatureStats
	{
		public class LifeStats
		{
			private int _currentHp, _maximumHp;
			private int _currentEnergy, _maximumEnergy;

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

			public int CurrentEnergy
			{
				get
				{
					return _currentEnergy;
				}

				set
				{
					if (_currentEnergy == value)
					{
						return;
					}

					_currentEnergy = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public int MaximumEnergy
			{
				get
				{
					return _maximumEnergy;
				}

				set
				{
					if (_maximumEnergy == value)
					{
						return;
					}

					_maximumEnergy = value;
					Changed?.Invoke(this, EventArgs.Empty);
				}
			}

			public event EventHandler Changed;

			public void Restore()
			{
				CurrentHP = MaximumHP;
				CurrentEnergy = MaximumEnergy;
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
