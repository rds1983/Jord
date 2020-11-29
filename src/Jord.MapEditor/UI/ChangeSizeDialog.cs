using Myra.Graphics2D.UI;
using System;
using Jord.Core;

namespace Jord.MapEditor.UI
{
	public enum ChangeSizeAction
	{
		Expand,
		Cut
	}

	public enum ChangeSizeDirection
	{
		Left,
		Top,
		Right,
		Bottom
	}

	public partial class ChangeSizeDialog
	{
		private Map _map;

		public Map Map
		{
			get
			{
				return _map;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (_map == value)
				{
					return;
				}

				_map = value;
				_labelCurrentSize.Text = string.Format("{0}x{1}", _map.Width, _map.Height);
			}
		}

		public ChangeSizeAction Action
		{
			get => (ChangeSizeAction)_comboAction.SelectedIndex.Value;
		}

		public ChangeSizeDirection Direction
		{
			get => (ChangeSizeDirection)_comboDirection.SelectedIndex.Value;
		}

		public int Amount
		{
			get => (int)_numericAmount.Value.Value;
		}

		public TileInfo FillerTile
		{
			get => (TileInfo)_comboFill.SelectedItem.Tag;
		}

		public ChangeSizeDialog()
		{
			BuildUI();

			foreach (var tileInfo in TJ.Module.TileInfos)
			{
				var text = tileInfo.Key;
				_comboFill.Items.Add(new ListItem(text, null, tileInfo.Value));
			}

			_comboAction.SelectedIndexChanged += _comboAction_SelectedIndexChanged;

			_comboAction.SelectedIndex = 0;
			_comboDirection.SelectedIndex = 0;
			_comboFill.SelectedIndex = 0;
		}

		private void _comboAction_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateVisible();
		}

		private void UpdateVisible()
		{
			var fillVisible = _comboAction.SelectedIndex == 0;
			_labelFill.Visible = fillVisible;
			_comboFill.Visible = fillVisible;
		}
	}
}