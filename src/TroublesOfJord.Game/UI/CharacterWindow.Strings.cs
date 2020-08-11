namespace TroublesOfJord.UI
{
	partial class CharacterWindow
	{
		public const string NonPrimaryLevelHigher = "Non-primary class level can't be higher than primary.";
		public const string PrimaryLevelLower = "Primary class level can't be lower than non-primary.";

		public static string ClassPointsLeft(int points)
		{
			return string.Format("You have \\c[green]{0} \\c[white]points left to distribute.", points);
		}
	}
}
