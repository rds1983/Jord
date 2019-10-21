using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace Wanderers.UI
{
	public class GameView: SingleItemContainer<HorizontalStackPanel>
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

			InternalChild = new HorizontalStackPanel();

			InternalChild.Proportions.Add(new Proportion(ProportionType.Pixels, MapRender.TileSize.X * 25));
			InternalChild.Proportions.Add(Proportion.Auto);
			InternalChild.Proportions.Add(Proportion.Fill);

			InternalChild.Widgets.Add(MapView);
			InternalChild.Widgets.Add(new VerticalSeparator());
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
