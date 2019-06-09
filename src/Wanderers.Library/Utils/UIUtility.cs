using Myra;
using Myra.Graphics2D.UI;

namespace Wanderers.Utils
{
	public static class UIUtility
	{
		public static void SetGridStyle(this Grid grid)
		{
			grid.SelectionBackground = DefaultAssets.UISpritesheet["tree-selection"];
			grid.SelectionHoverBackground = DefaultAssets.UISpritesheet["button-over"];
		}
	}
}