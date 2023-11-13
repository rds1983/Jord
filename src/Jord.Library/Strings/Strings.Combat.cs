using System;
using System.Text;
using Jord.Core;

namespace Jord
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
			_attackNames[(int)AttackType.Bash] = new AttackNames("bash", "bashes");
			_attackNames[(int)AttackType.Claw] = new AttackNames("claw", "claws");
			_attackNames[(int)AttackType.Bite] = new AttackNames("bite", "bites");
			_attackNames[(int)AttackType.Smash] = new AttackNames("smash", "smashes");
		}

		public static string GetAttackNoun(AttackType attackType)
		{
			return _attackNames[(int)attackType].Noun;
		}

		public static string GetAttackVerb(AttackType attackType)
		{
			return _attackNames[(int)attackType].Verb;
		}

		public static string GetEvadeMessage(string attackerName, string targetName, AttackType attackType)
		{
			return $"{targetName} evades {GetAttackNoun(attackType)} of {attackerName}.";
		}

		public static string GetArmorMessage(string attackerName, string targetName, AttackType attackType, bool blocked)
		{
			var sb = new StringBuilder();
			if (blocked)
			{
				sb.Append("Blocked. ");
			}

			sb.Append($"{attackerName} couldn't pierce through the armor of {targetName} with {GetAttackNoun(attackType)}.");
			
			return sb.ToString();
		}

		public static string GetAttackMessage(int damage, string attackerName, string targetName, AttackType attackType, bool blocked)
		{
			var sb = new StringBuilder();
			if (blocked)
			{
				sb.Append("Blocked. ");
			}
			if (damage < 5)
			{
				sb.Append($"{attackerName} barely {GetAttackVerb(attackType)} {targetName} ({damage}).");
			}
			else if (damage < 10)
			{
				sb.Append($"{attackerName} {GetAttackVerb(attackType)} {targetName} ({damage}).");
			}
			else if (damage < 15)
			{
				sb.Append($"{attackerName} {GetAttackVerb(attackType)} {targetName} hard ({damage}).");
			}
			else if (damage < 20)
			{
				sb.Append($"{attackerName} {GetAttackVerb(attackType)} {targetName} very hard ({damage}).");
			}
			else if (damage < 25)
			{
				sb.Append($"{attackerName} {GetAttackVerb(attackType)} {targetName} extremelly hard ({damage}).");
			}
			else if (damage < 30)
			{
				sb.Append($"{attackerName} massacres {targetName} to small fragments with {GetAttackNoun(attackType)} ({damage}).");
			}
			else if (damage < 50)
			{
				sb.Append($"{attackerName} brutally massacres {targetName} to small fragments with {GetAttackNoun(attackType)} ({damage}).");
			}
			else
			{
				sb.Append($"{attackerName} viciously massacres {targetName} to small fragments with {GetAttackNoun(attackType)} ({damage}).");
			}

			return sb.ToString();
		}

		public static string GetNpcDeathMessage(string name)
		{
			return string.Format("{0} is dead! R.I.P. Your blood freezes as you hear {0}'s death cry.", name);
		}

		public static string GetExpMessage(int experience)
		{
			return string.Format("You had been awarded {0} experience.", experience);
		}
	}
}
