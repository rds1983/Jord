namespace Jord.UI
{
	public enum CharacterWindowTab
	{
		Stats,
		Perks,
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

			var characterPerksPanel = new CharacterPerksPanel();
			_tabItemPerks.Content = characterPerksPanel;

			var characterInventoryPanel = new CharacterInventoryPanel();
			_tabItemInventory.Content = characterInventoryPanel;
		}
	}
}