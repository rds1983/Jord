/* Generated by Myra UI Editor at 7/24/2017 10:46:51 PM */
using Myra.Graphics2D.UI;

namespace Jord.MapEditor.UI
{
	public partial class StudioWidget: Grid
	{
		public readonly MapEditor _mapEditor;
		public readonly MapNavigation _mapNavigation;

		public StudioWidget()
		{
			BuildUI();

			_mapEditor = new MapEditor();

			_mapNavigation = new MapNavigation
			{
				IgnoreFov = true
			};

			_leftContainer.Widgets.Add(_mapEditor);
			_mapViewerContainer.Widgets.Add(_mapNavigation);

			_mapNavigation.MapEditor = _mapEditor;
		}
	}
}