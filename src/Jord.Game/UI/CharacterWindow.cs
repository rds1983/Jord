namespace Jord.UI
{
	public partial class CharacterWindow
	{
		public CharacterWindow()
		{
			BuildUI();

			var player = TJ.Player;

			var nextLevel = TJ.Database.LevelCosts[player.Level + 1];
			var title = $"{player.Name}, {player.Class}, {player.Level}, {player.Experience.FormatNumber()}/{nextLevel.Experience.FormatNumber()}";
			Title = title;
		}
	}
}