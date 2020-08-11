namespace TroublesOfJord
{
	public static partial class Strings
	{
		public const string EmptySlotName = " --EMPTY-- ";
		public const string BackName = " --BACK-- ";
		public const string InstructorCaption = "Instructor";
		public const string ReachedMaximumLevel = "You had reached the maximum level.";

		public const string Error = "Error";

		public static string FormatNumber(int number)
		{
			return number.ToString();
		}

		public static string BuildNextLevelRequirements(int experience, int gold,
			int nextLevelExperience, int nextLevelGold)
		{
			return string.Format("Your experience: {0}, your gold: {1}.\nYou need {2} experience and {3} gold for the next level.",
				experience, gold,
				nextLevelExperience, nextLevelGold);
		}

		public static string BuildNextLevelOffer(int experience, int gold,
			int nextLevelExperience, int nextLevelGold)
		{
			return string.Format("Your experience: {0}, your gold: {1}.\nWould you like to gain next level for {2} experience and {3} gold?",
				experience, gold,
				nextLevelExperience, nextLevelGold);
		}

		public static string BuildNextLevel(int newLevel)
		{
			return string.Format("Welcome to the level {0}!", newLevel);
		}
	}
}