/* Generated by MyraPad at 4/10/2023 8:17:16 PM */
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Properties;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jord.MapEditor.UI
{
	partial class NewMapDialog: Dialog
	{
		private void BuildUI()
		{
			var label1 = new Label();
			label1.Text = "Id:";

			_textId = new TextBox();
			_textId.GridColumn = 1;
			_textId.Id = "_textId";

			var label2 = new Label();
			label2.Text = "Width:";
			label2.GridRow = 1;

			_spinButtonWidth = new SpinButton();
			_spinButtonWidth.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
			_spinButtonWidth.Value = 32;
			_spinButtonWidth.GridColumn = 1;
			_spinButtonWidth.GridRow = 1;
			_spinButtonWidth.Id = "_spinButtonWidth";

			var label3 = new Label();
			label3.Text = "Height:";
			label3.GridRow = 2;

			_spinButtonHeight = new SpinButton();
			_spinButtonHeight.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
			_spinButtonHeight.Value = 32;
			_spinButtonHeight.GridColumn = 1;
			_spinButtonHeight.GridRow = 2;
			_spinButtonHeight.Id = "_spinButtonHeight";

			var label4 = new Label();
			label4.Text = "Tile:";
			label4.GridRow = 3;

			_comboTile = new ComboBox();
			_comboTile.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Stretch;
			_comboTile.GridColumn = 1;
			_comboTile.GridRow = 3;
			_comboTile.Id = "_comboTile";

			var grid1 = new Grid();
			grid1.ColumnSpacing = 8;
			grid1.RowSpacing = 8;
			grid1.DefaultRowProportion = new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			};
			grid1.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Auto,
			});
			grid1.ColumnsProportions.Add(new Proportion
			{
				Type = Myra.Graphics2D.UI.ProportionType.Fill,
			});
			grid1.Widgets.Add(label1);
			grid1.Widgets.Add(_textId);
			grid1.Widgets.Add(label2);
			grid1.Widgets.Add(_spinButtonWidth);
			grid1.Widgets.Add(label3);
			grid1.Widgets.Add(_spinButtonHeight);
			grid1.Widgets.Add(label4);
			grid1.Widgets.Add(_comboTile);

			
			Title = "New Map";
			Left = 808;
			Top = 203;
			Width = 300;
			Content = grid1;
		}

		
		public TextBox _textId;
		public SpinButton _spinButtonWidth;
		public SpinButton _spinButtonHeight;
		public ComboBox _comboTile;
	}
}
