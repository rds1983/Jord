namespace Jord.UI
{
	public enum CharacterWindowTab
	{
		Stats,
		Inventory
	}

	public partial class CharacterWindow
	{
		public CharacterWindowTab CurrentTab
		{
			get => (CharacterWindowTab)_tabControlMain.SelectedIndex;
			set => _tabControlMain.SelectedIndex = (int)value;
		}

		public CharacterWindow()
		{
			BuildUI();

			var player = TJ.Player;

			var nextLevel = TJ.Database.LevelCosts[player.Level + 1];
			var title = $"{player.Name}, {player.Class}, {player.Level}, {player.Experience.FormatNumber()}/{nextLevel.Experience.FormatNumber()}";
			Title = title;

			var characterStatsPanel = new CharacterStatsPanel();
			_tabItemStats.Content = characterStatsPanel;

			var characterInventoryPanel = new CharacterInventoryPanel();
			_tabItemInventory.Content = characterInventoryPanel;
		}
	}
}