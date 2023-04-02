/* Generated by MyraPad at 8/20/2020 4:48:54 PM */

using System;
using Myra.Graphics2D.UI;
using Jord.Core.Abilities;

namespace Jord.UI
{
	public partial class AbilitiesWindow
	{
		public AbilitiesWindow()
		{
			BuildUI();
			RebuildAbilities();
			UpdateSelected();

			_buttonUse.Click += _buttonUse_Click;
		}

		private void _buttonUse_Click(object sender, EventArgs e)
		{
			var ability = (AbilityInfo)_listAbilities.SelectedItem.Tag;
			TJ.UseAbility(ability);
			Close();
		}

		private void RebuildAbilities()
		{
			var player = TJ.Player;

			_listAbilities.Items.Clear();

			foreach(var ability in player.Abilities)
			{
				_listAbilities.Items.Add(new ListItem(ability.Name, null, ability));
			}

			_listAbilities.SelectedIndexChanged += ListAbilitiesOnSelectedIndexChanged;
		}

		private void ListAbilitiesOnSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateSelected();
		}

		private void UpdateSelected()
		{
			if (_listAbilities.SelectedItem == null)
			{
				_labelEnergyCost.Text = string.Empty;
				_labelType.Text = string.Empty;
				_labelDescription.Text = string.Empty;
				_buttonUse.Enabled = false;
				return;
			}

			var ability = (AbilityInfo) _listAbilities.SelectedItem.Tag;
			_labelEnergyCost.Text = GetEnergyString(ability.Mana);
			_labelType.Text = GetTypeString(ability.Type);
			_labelDescription.Text = ability.Description;
			_buttonUse.Enabled = ability.Type == AbilityType.Instant;
		}
	}
}