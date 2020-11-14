using System;
using TroublesOfJord.Core;

namespace TroublesOfJord
{
	static partial class Strings
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

		private static readonly AttackNames[] _attackNames = new AttackNames[Enum.GetNames(typeof(AttackType)).Length];

		static Strings()
		{
			_attackNames[(int)AttackType.Hit] = new AttackNames("hit", "hits");
			_attackNames[(int)AttackType.Slash] = new AttackNames("slash", "slashes");
			_attackNames[(int)AttackType.Claw] = new AttackNames("claw", "claws");
		}

		public static string GetAttackNoun(AttackType attackType)
		{
			return _attackNames[(int)attackType].Noun;
		}

		public static string GetAttackVerb(AttackType attackType)
		{
			return _attackNames[(int)attackType].Verb;
		}

		public static string GetMissMessage(string attackerName, string targetName, AttackType attackType)
		{
			return string.Format("{0} misses {1} with {2}.",
				attackerName, targetName, GetAttackNoun(attackType));
		}

		public static string GetAttackMessage(int damage, string attackerName, string targetName, AttackType attackType)
		{
			if (damage < 5)
			{
				return string.Format("{0} barely {1} {2} ({3}).",
					attackerName, GetAttackVerb(attackType), targetName, damage);
			}
			else if (damage < 10)
			{
				return string.Format("{0} {1} {2} ({3}).",
					attackerName, GetAttackVerb(attackType), targetName, damage);
			}
			else if (damage < 15)
			{
				return string.Format("{0} {1} {2} hard ({3}).",
					attackerName, GetAttackVerb(attackType), targetName, damage);
			}
			else if (damage < 20)
			{
				return string.Format("{0} {1} {2} very hard ({3}).",
					attackerName, GetAttackVerb(attackType), targetName, damage);
			}
			else if (damage < 25)
			{
				return string.Format("{0} {1} {2} extremelly hard ({3}).",
					attackerName, GetAttackVerb(attackType), targetName, damage);
			}
			else if (damage < 30)
			{
				return string.Format("{0} massacres {1} to small fragments with {2} ({3}).",
					attackerName, targetName, GetAttackNoun(attackType), damage);
			}
			else if (damage < 50)
			{
				return string.Format("{0} brutally massacres {1} to small fragments with {2} ({3}).",
					attackerName, targetName, GetAttackNoun(attackType), damage);
			}

			return string.Format("{0} viciously massacres {1} to small fragments with {2} ({3}).",
				attackerName, targetName, GetAttackNoun(attackType), damage);
		}

		public static string GetNpcDeathMessage(string name)
		{
			return string.Format("{0} is dead! R.I.P. Your blood freezes as you hear {0}'s death cry.", name);
		}

		public static string GetExpGoldMessage(int experience, int gold)
		{
			return string.Format("You had been awarded {0} experience and {1} gold.", experience, gold);
		}
	}
}
