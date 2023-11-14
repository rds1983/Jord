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
			_labelMeleeMastery.Text = stats.MeleeMastery.ToString();
			_labelArmorRating.Text = stats.ArmorRating.ToString();
			_labelEvasionRating.Text = stats.EvasionRating.ToString();
			_labelBlockingRating.Text = stats.BlockingRating.ToString();
		}
	}
}