using Jord.Core.Items;
using Jord.Core;
using Jord.Utils;
using Myra.Graphics2D.UI;
using System.Text;
using System;
using Microsoft.Xna.Framework.Input;

namespace Jord.UI
{
	public partial class CharacterInventoryPanel
	{
		public Player Player
		{
			get
			{
				return TJ.Session.Player;
			}
		}

		private Item SelectedItem
		{
			get
			{
				Item item = null;
				if (_gridEquipment.SelectedRowIndex != null)
				{
					item = Player.Equipment.Items[_gridEquipment.SelectedRowIndex.Value].Item;
				}
				else if (_gridInventory.SelectedRowIndex != null)
				{
					item = Player.Inventory.Items[_gridInventory.SelectedRowIndex.Value].Item;
				}

				return item;
			}
		}

		public CharacterInventoryPanel()
		{
			BuildUI();

			AcceptsKeyboardFocus = true;

			_gridEquipment.SetGridStyle();
			_gridInventory.SetGridStyle();

			_gridEquipment.HoverIndexChanged += OnEquipmentHoverIndexChanged;
			_gridEquipment.SelectedIndexChanged += OnEquipmentSelectedIndexChanged;

			_gridInventory.HoverIndexChanged += OnInventoryHoverIndexChanged;
			_gridInventory.SelectedIndexChanged += OnInventorySelectedIndexChanged;

			_textDescription.Text = string.Empty;

			_buttonEquip.Click += _buttonEquip_Click;
			_buttonDrop.Click += _buttonDrop_Click;

			Rebuild();
		}

		private void _buttonDrop_Click(object sender, EventArgs e)
		{
			var item = SelectedItem;

			if (_gridEquipment.SelectedRowIndex != null)
			{
				// Remove from equipment
				Player.Equipment.Remove(_gridEquipment.SelectedRowIndex.Value);
			}
			else if (_gridInventory.SelectedRowIndex != null)
			{
				// Remove from inventory
				Player.Inventory.Add(item, -1);
			}

			// Drop on tile
			Player.Tile.Inventory.Add(item, 1);

			Rebuild();
		}

		private void _buttonEquip_Click(object sender, EventArgs e)
		{
			var item = SelectedItem;

			if (_gridEquipment.SelectedRowIndex != null)
			{
				// Remove from equipment
				Player.Equipment.Remove(_gridEquipment.SelectedRowIndex.Value);

				// Add to inventory
				Player.Inventory.Add(item, 1);
			}
			else if (_gridInventory.SelectedRowIndex != null)
			{
				// Wear
				Player.Equipment.Equip(item);

				// Remove from inventory
				Player.Inventory.Add(item, -1);
			}

			Rebuild();
		}

		private void UpdateButtons()
		{
			_buttonDrop.Enabled = false;
			_buttonEquip.Enabled = false;
			_buttonUse.Enabled = false;

			var item = SelectedItem;
			if (item == null)
			{
				return;
			}

			var asEquip = item.Info as EquipInfo;
			if (asEquip != null)
			{
				if (_gridEquipment.SelectedRowIndex != null)
				{
					_buttonEquip.Text = string.Format(@"Un/c[green]e/c[white]quip");
				}
				else
				{
					_buttonEquip.Text = string.Format(@"/c[green]E/c[white]quip");
				}

				_buttonEquip.Enabled = true;
			}

			_buttonDrop.Enabled = true;
		}

		private void UpdateStats()
		{
			var battle = Player.Stats.Battle;
			_textAc.Text = "AC: " + battle.ArmorClass.ToString();
			_textHitRoll.Text = "Hit Roll: " + battle.HitRoll;

			var sb = new StringBuilder();

			sb.Append("Attacks: ");
			for (var i = 0; i < battle.Attacks.Length; ++i)
			{
				var attack = battle.Attacks[i];

				sb.Append(attack.MinDamage);
				sb.Append('-');
				sb.Append(attack.MaxDamage);

				if (i < Player.Stats.Battle.Attacks.Length - 1)
				{
					sb.Append('/');
				}
			}

			_textAttacks.Text = sb.ToString();
			_textGold.Text = "Gold: " + Player.Gold;
		}

		private void Rebuild()
		{
			_gridEquipment.Widgets.Clear();
			_gridEquipment.RowsProportions.Clear();

			foreach (var item in Player.Equipment.Items)
			{
				var row = _gridEquipment.RowsProportions.Count;

				var textSlot = new Label
				{
					Text = "<" + item.Slot.ToString().ToLower() + ">",
					GridRow = row
				};
				_gridEquipment.Widgets.Add(textSlot);

				if (item.Item != null)
				{
					var textItem = new Label
					{
						Text = item.Item.Info.Name,
						GridColumn = 1,
						GridRow = row
					};

					_gridEquipment.Widgets.Add(textItem);
				}

				_gridEquipment.RowsProportions.Add(new Proportion());
			}

			_gridInventory.Widgets.Clear();
			_gridInventory.RowsProportions.Clear();

			foreach (var item in Player.Inventory.Items)
			{
				var row = _gridInventory.RowsProportions.Count;

				var textSlot = new Label
				{
					Text = item.ToString(),
					GridRow = row
				};
				_gridInventory.Widgets.Add(textSlot);

				_gridInventory.RowsProportions.Add(new Proportion());
			}

			UpdateStats();
			UpdateButtons();
		}

		private void OnHoverIndexChanged(Grid grid, Item item)
		{
			if (grid.HoverRowIndex == null)
			{
				return;
			}

			_textDescription.Text = item.BuildDescription();
		}

		private void OnInventoryHoverIndexChanged(object sender, EventArgs e)
		{
			if (_gridInventory.HoverRowIndex == null)
			{
				return;
			}

			var item = Player.Inventory.Items[_gridInventory.HoverRowIndex.Value].Item;
			_textDescription.Text = item.BuildDescription();
		}

		private void OnEquipmentHoverIndexChanged(object sender, EventArgs e)
		{
			if (_gridEquipment.HoverRowIndex == null)
			{
				return;
			}

			var item = Player.Equipment.Items[_gridEquipment.HoverRowIndex.Value].Item;
			_textDescription.Text = item != null ? item.BuildDescription() : string.Empty;
		}

		private void OnInventorySelectedIndexChanged(object sender, EventArgs e)
		{
			if (_gridInventory.SelectedRowIndex == null)
			{
				return;
			}

			_gridEquipment.SelectedRowIndex = null;

			UpdateButtons();
		}

		private void OnEquipmentSelectedIndexChanged(object sender, EventArgs e)
		{
			if (_gridEquipment.SelectedRowIndex == null)
			{
				return;
			}

			_gridInventory.SelectedRowIndex = null;

			UpdateButtons();
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.E:
					if (_buttonEquip.Enabled)
					{
						_buttonEquip.DoClick();
					}
					break;
				case Keys.D:
					if (_buttonDrop.Enabled)
					{
						_buttonDrop.DoClick();
					}
					break;
			}
		}
	}
}