/* Generated by MyraPad at 08.03.2019 2:13:39 */
using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.UI;
using System;
using Wanderers.Core;
using Wanderers.Core.Items;
using Wanderers.Utils;

namespace Wanderers.UI
{
	public partial class TradeDialog: Dialog
	{
		private const float SellK = 0.5f;

		private readonly Creature _left, _right;
		private readonly Inventory _leftInventory = new Inventory();
		private readonly Inventory _leftAddition = new Inventory();
		private readonly Inventory _rightInventory = new Inventory();
		private readonly Inventory _rightAddition = new Inventory();
		private readonly Arrow _arrow = new Arrow();
		private int _leftToRightGoldTransfer = 0;

		public TradeDialog(Creature left, Creature right)
		{
			if (left == null)
			{
				throw new ArgumentNullException(nameof(left));
			}

			if (right == null)
			{
				throw new ArgumentNullException(nameof(right));
			}

			_left = left;
			_right = right;

			BuildUI();

			_panelArrow.Widgets.Add(_arrow);

			_gridLeft.HoverIndexChanged += OnLeftHoverIndexChanged;
			_gridLeft.SelectedIndexChanged += OnLeftSelectedIndexChanged;

			_gridRight.HoverIndexChanged += OnRightHoverIndexChanged;
			_gridRight.SelectedIndexChanged += OnRightSelectedIndexChanged;

			_gridLeft.SetGridStyle();
			_gridRight.SetGridStyle();

			_textNameLeft.Text = _left.Name;
			_textGoldLeft.Text = _left.Gold.ToString();

			_textNameRight.Text = _right.Name;
			_textGoldRight.Text = _right.Gold.ToString();

			// Fill initial inventories
			foreach (var itemPile in _left.Inventory.Items)
			{
				_leftInventory.Items.Add(new ItemPile
				{
					Item = itemPile.Item,
					Quantity = itemPile.Quantity
				});
			}

			foreach (var itemPile in _right.Inventory.Items)
			{
				_rightInventory.Items.Add(new ItemPile
				{
					Item = itemPile.Item,
					Quantity = itemPile.Quantity
				});
			}

			Update();
		}

		private void OnSelectedIndexChanged(Grid grid, Inventory inventory, Inventory addition, Inventory oppositeInventory, Inventory oppositeAddition)
		{
			if (grid.SelectedRowIndex == null)
			{
				return;
			}

			var index = grid.SelectedRowIndex.Value;
			if (index < inventory.Items.Count)
			{
				var itemPile = inventory.Items[index];
				var quantity = itemPile.Quantity;
				inventory.Add(itemPile.Item, -quantity);
				oppositeAddition.Add(itemPile.Item, quantity);
			}
			else
			{
				var itemPile = addition.Items[index - inventory.Items.Count];
				var quantity = itemPile.Quantity;
				oppositeInventory.Add(itemPile.Item, quantity);
				addition.Add(itemPile.Item, -quantity);
			}

			grid.SelectedRowIndex = null;

			Update();
		}

		private void OnRightSelectedIndexChanged(object sender, EventArgs e)
		{
			OnSelectedIndexChanged(_gridRight, _rightInventory, _rightAddition, _leftInventory, _leftAddition);
		}

		private void OnLeftSelectedIndexChanged(object sender, EventArgs e)
		{
			OnSelectedIndexChanged(_gridLeft, _leftInventory, _leftAddition, _rightInventory, _rightAddition);
		}

		private void OnHoverIndexChanged(Grid grid, Inventory inventory, Inventory addition)
		{
			if (grid.HoverRowIndex == null)
			{
				return;
			}

			var index = grid.HoverRowIndex.Value;
			ItemPile itemPile = null;
			if (index < inventory.Items.Count)
			{
				itemPile = inventory.Items[index];
			}
			else
			{
				itemPile = addition.Items[index - inventory.Items.Count];
			}

			_textDescription.Text = itemPile.Item.BuildDescription();
		}

		private void OnRightHoverIndexChanged(object sender, EventArgs e)
		{
			OnHoverIndexChanged(_gridRight, _rightInventory, _rightAddition);
		}

		private void OnLeftHoverIndexChanged(object sender, EventArgs e)
		{
			OnHoverIndexChanged(_gridLeft, _leftInventory, _leftAddition);
		}

		private int GetItemPrice(Item item, int quantity, float k = 1.0f)
		{
			var price = item.Info.Price;
			price = (int)(price * k);

			if (price <= 0)
			{
				price = 1;
			}

			price *= quantity;

			return price;
		}

		private void AddGridPosition(Grid grid, ItemPile itemPile, bool isAddition)
		{
			var row = grid.RowsProportions.Count;

			grid.RowsProportions.Add(new Grid.Proportion());

			if (isAddition)
			{
				var image = new Image
				{
					Renderable = DefaultAssets.UISpritesheet["vis-check-tick"],
					GridRow = row,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};

				grid.Widgets.Add(image);
			}

			var text = itemPile.Item.Info.Name;

			if (itemPile.Quantity > 1)
			{
				text = itemPile.ToString();
			}

			var textBlock = new TextBlock
			{
				Text = text,
				GridColumn = 1,
				GridRow = row
			};

			grid.Widgets.Add(textBlock);

			var isSell = grid == _gridLeft;
			if (isAddition)
			{
				isSell = !isSell;
			}

			var price = GetItemPrice(itemPile.Item, 1, isSell ? SellK : 1.0f);

			textBlock = new TextBlock
			{
				Text = price.ToString(),
				GridColumn = 2,
				GridRow = row
			};

			grid.Widgets.Add(textBlock);
		}

		private void FillGrid(Grid grid, Inventory inventory, Inventory addition)
		{
			grid.Widgets.Clear();
			grid.RowsProportions.Clear();

			for (var i = 0; i < inventory.Items.Count; ++i)
			{
				var itemPile = inventory.Items[i];
				AddGridPosition(grid, itemPile, false);
			}

			for (var i = 0; i < addition.Items.Count; ++i)
			{
				var itemPile = addition.Items[i];
				AddGridPosition(grid, itemPile, true);
			}
		}

		private void Update()
		{
			FillGrid(_gridLeft, _leftInventory, _leftAddition);
			FillGrid(_gridRight, _rightInventory, _rightAddition);

			_leftToRightGoldTransfer = 0;
			foreach (var pos in _leftAddition.Items)
			{
				_leftToRightGoldTransfer += GetItemPrice(pos.Item, pos.Quantity);
			}

			foreach (var pos in _rightAddition.Items)
			{
				_leftToRightGoldTransfer -= GetItemPrice(pos.Item, pos.Quantity, SellK);
			}

			if (_leftToRightGoldTransfer == 0)
			{
				_gridGoldTransfer.Visible = false;
			}
			else
			{
				_gridGoldTransfer.Visible = true;
				_textGoldTransfer.Text = Resources.TextGold +  ": " + Math.Abs(_leftToRightGoldTransfer).ToString();
				_textGoldTransfer.TextColor = _leftToRightGoldTransfer > _left.Gold ? Color.Red : Color.White;
				_arrow.Direction = _leftToRightGoldTransfer > 0 ? ArrowDirection.Right : ArrowDirection.Left;
			}

			ButtonOk.Enabled = _leftAddition.Items.Count > 0 || _rightAddition.Items.Count > 0;
		}

		protected override bool CanCloseByOk()
		{
			if (_leftToRightGoldTransfer <= _left.Gold)
			{
				foreach (var pos in _leftAddition.Items)
				{
					_left.Inventory.Add(pos.Item, pos.Quantity);
					_right.Inventory.Add(pos.Item, -pos.Quantity);
				}

				foreach (var pos in _rightAddition.Items)
				{
					_left.Inventory.Add(pos.Item, -pos.Quantity);
					_right.Inventory.Add(pos.Item, pos.Quantity);
				}

				_left.Gold -= _leftToRightGoldTransfer;
				_right.Gold += _leftToRightGoldTransfer;

				return true;
			}

			var msg = Dialog.CreateMessageBox(Resources.TextNotEnoughGold, Resources.TextNotEnoughGold);
			msg.ShowModal(Desktop);

			return false;
		}
	}
}