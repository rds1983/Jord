using TroublesOfJord.Core.Abilities;

namespace TroublesOfJord.UI
{
    partial class AbilitiesWindow
    {
        private const string ManualNone = "No manual required";
        
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