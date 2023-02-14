namespace Jord
{
	public static partial class Strings
	{
		public const string EmptySlotName = " --EMPTY-- ";
		public const string BackName = " --BACK-- ";
		public const string InstructorCaption = "Instructor";
		public const string ReachedMaximumLevel = "You had reached the maximum level.";
		public const string MaximumHpAlready = "Your hitpoints are at maximum already.";
		public const string NotEnoughEnergy = "Not enough energy.";
		public const string SomeItemsAreLying = "Some items are lying on the floor.";

		public const string Confirm = "Confirm";
		public const string Error = "Error";

		public static string FormatNumber(this int number)
		{
			return number.ToString();
		}

		public static string BuildNextLevel(int newLevel, int classPointsLeft)
		{
			return string.Format("Welcome to the level {0}! 1 class point had been awarded. Total class points: {1}.", newLevel, classPointsLeft);
		}

		public static string BuildRushesToAttack(string name)
		{
			return string.Format("{0} sees you and rushes to attack!", name);
		}

		public static string BuildEnteredMap(string name)
		{
			return string.Format("You've entered '{0}'.", name);
		}

		public static string BuildItemLyingOnTheFloor(string name)
		{
			return string.Format("{0} is lying on the floor.", name);
		}

		public static string BuildPickedUp(string name, int amount)
		{
			if (amount == 1)
			{
				return string.Format("You picked {0} from the floor.", name);
			}

			return string.Format("You picked {0} ({1}) from the floor.", name, amount);
		}
	}
}