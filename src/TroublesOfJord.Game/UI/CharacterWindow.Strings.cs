namespace TroublesOfJord.UI
{
	partial class CharacterWindow
	{
		private const string NonPrimaryLevelHigher = "Non-primary class level can't be higher than primary.";
		private const string PrimaryLevelLower = "Primary class level can't be lower than non-primary.";
		private const string NoPointsLeft = "You have no points left to distribute.";
		private const string Confirm = "Do you confirm the distribution of points?";

		private static string ClassPointsLeft(int points)
		{
			return string.Format("You have \\c[green]{0} \\c[white]points left to distribute.", points);
		}

		private static string Experience(string value)
		{
			return "Experience: " + value;
		}

		private static string Gold(string value)
		{
			return "Gold: " + value;
  		}
	}
}
