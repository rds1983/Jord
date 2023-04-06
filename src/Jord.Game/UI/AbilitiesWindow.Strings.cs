using Jord.Core.Abilities;

namespace Jord.UI
{
	partial class AbilitiesWindow
	{
		private static string GetEnergyString(int value)
		{
			return value + " " + "Energy";
		}

		private static string GetTypeString(AbilityType type)
		{
			return type.ToString();
		}
	}
}