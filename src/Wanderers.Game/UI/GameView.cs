using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace Wanderers.UI
{
	public class GameView: SingleItemContainer<Panel>
	{
		private readonly MapView _mapView = new MapView();

		public MapView MapView
		{
			get
			{
				return _mapView;
			}
		}

		public GameView()
		{
			InternalChild = new Panel();
			InternalChild.Widgets.Add(MapView);

			CanFocus = true;
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			var inventoryWindow = new InventoryWindow();
			inventoryWindow.ShowModal(Desktop);
		}
	}
}
