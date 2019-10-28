using Microsoft.Xna.Framework.Input;

namespace Wanderers.UI
{
	public partial class GameView
	{
		public MapView MapView { get; } = new MapView();
		public MiniMap MapNavigation { get; } = new MiniMap();
		public LogView LogView { get; } = new LogView();

		protected override bool AcceptsKeyboardFocus => true;

		public GameView()
		{
			BuildUI();

			_mapViewContainer.Widgets.Add(MapView);

			MapNavigation.MapEditor = MapView;
			_mapContainer.Widgets.Add(MapNavigation);

			_logContainer.Widgets.Add(LogView);
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (k == Keys.I)
			{
				var inventoryWindow = new InventoryWindow();
				inventoryWindow.ShowModal(Desktop);
			}

			if (k == Keys.E)
			{
				if (TJ.Session.Player.Enter())
				{
					MapNavigation.InvalidateImage();
				}
			}
		}
	}
}