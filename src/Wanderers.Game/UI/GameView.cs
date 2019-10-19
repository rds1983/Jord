using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace Wanderers.UI
{
	public class GameView: SingleItemContainer<Grid>
	{
		private readonly MapView _mapView = new MapView();
		private readonly LogView _logView = new LogView();

		public MapView MapView
		{
			get
			{
				return _mapView;
			}
		}

		public LogView LogView
		{
			get
			{
				return _logView;
			}
		}

		protected override bool AcceptsKeyboardFocus => true;

		public GameView()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			InternalChild = new Grid();

			InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, MapRender.TileSize.X * 25));
			InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			InternalChild.Widgets.Add(MapView);

			_logView.GridColumn = 1;
			InternalChild.Widgets.Add(_logView);
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (k == Keys.I)
			{
				var inventoryWindow = new InventoryWindow();
				inventoryWindow.ShowModal(Desktop);
			}
		}
	}
}
