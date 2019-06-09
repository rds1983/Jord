/* Generated by MyraPad at 09.06.2019 15:38:52 */
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace Wanderers.MapEditor.UI
{
	partial class NewMapDialog
	{
		private void BuildUI()
		{
			_radioSingleTileMap = new RadioButton();
			_radioSingleTileMap.Text = "Single Tile Map";
			_radioSingleTileMap.Id = "_radioSingleTileMap";

			_radioGeneratedGlobalMap = new RadioButton();
			_radioGeneratedGlobalMap.Text = "Generated Global Map";
			_radioGeneratedGlobalMap.Id = "_radioGeneratedGlobalMap";
			_radioGeneratedGlobalMap.GridRow = 1;

			var grid1 = new Grid();
			grid1.RowsProportions.Add(new Grid.Proportion());
			grid1.RowsProportions.Add(new Grid.Proportion());
			grid1.GridRow = 1;
			grid1.Widgets.Add(_radioSingleTileMap);
			grid1.Widgets.Add(_radioGeneratedGlobalMap);

			
			Title = "New Map";
			Left = 391;
			Top = 132;
			Content = grid1;
		}

		
		public RadioButton _radioSingleTileMap;
		public RadioButton _radioGeneratedGlobalMap;
	}
}