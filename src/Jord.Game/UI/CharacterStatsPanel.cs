namespace Jord.UI
{
	public partial class CharacterStatsPanel
	{
		public CharacterStatsPanel()
		{
			BuildUI();
		}

		protected override void OnPlacedChanged()
		{
			base.OnPlacedChanged();

			if (!IsPlaced)
			{
				return;
			}

			var stats = TJ.Player.Stats.Battle;
			_labelMelee.Text = stats.Melee.ToString();
			_labelArmor.Text = stats.Armor.ToString();
			_labelEvasion.Text = stats.Evasion.ToString();
			_labelBlocking.Text = stats.Blocking.ToString();
			_labelAttacks.Text = stats.Attacks.Length.ToString();
		}
	}
}