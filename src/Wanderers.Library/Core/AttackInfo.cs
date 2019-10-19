using System;

namespace Wanderers.Core
{
	public enum AttackType
	{
		Hit,
		Slash,
		Claw
	}

	public class AttackInfo
	{
		private class AttackNames
		{
			public string Noun;
			public string Verb;

			public AttackNames(string noun, string verb)
			{
				Noun = noun;
				Verb = verb;
			}
		}

		public AttackType AttackType;
		public int MinDamage;
		public int MaxDamage;
		public int Delay;

		private static readonly AttackNames[] _attackNames = new AttackNames[Enum.GetNames(typeof(AttackType)).Length];

		static AttackInfo()
		{
			_attackNames[(int)AttackType.Hit] = new AttackNames("hit", "hits");
			_attackNames[(int)AttackType.Slash] = new AttackNames("slash", "slashes");
			_attackNames[(int)AttackType.Claw] = new AttackNames("claw", "claws");
		}

		public AttackInfo(AttackType attackType, int minDamage, int maxDamage, int delay)
		{
			AttackType = attackType;
			MinDamage = minDamage;
			MaxDamage = maxDamage;
			Delay = delay;
		}

		public AttackInfo()
		{
		}

		public static string GetAttackNoun(AttackType attackType)
		{
			return _attackNames[(int)attackType].Noun;
		}

		public static string GetAttackVerb(AttackType attackType)
		{
			return _attackNames[(int)attackType].Verb;
		}

		public static string GetAttackMessage(int damage, string attackerName, string targetName, AttackType attackType)
		{
			if (damage < 5)
			{
				return string.Format("{0} barely {1} {2}.", attackerName, GetAttackVerb(attackType), targetName);
			} else if (damage < 10)
			{
				return string.Format("{0} {1} {2}.", attackerName, GetAttackVerb(attackType), targetName);
			}
			else if (damage < 15)
			{
				return string.Format("{0} {1} {2} hard.", attackerName, GetAttackVerb(attackType), targetName);
			}
			else if (damage < 20)
			{
				return string.Format("{0} {1} {2} very hard.", attackerName, GetAttackVerb(attackType), targetName);
			}
			else if (damage < 25)
			{
				return string.Format("{0} {1} {2} extremelly hard.", attackerName, GetAttackVerb(attackType), targetName);
			}
			else if (damage < 30)
			{
				return string.Format("{0} massacres {1} to small fragments with his {2}.", attackerName, targetName, GetAttackNoun(attackType));
			}
			else if (damage < 50)
			{
				return string.Format("{0} brutally massacres {1} to small fragments with his {2}.", attackerName, targetName, GetAttackNoun(attackType));
			}

			return string.Format("{0} viciously massacres {1} to small fragments with his {2}.", attackerName, targetName, GetAttackNoun(attackType));
		}
	}
}
