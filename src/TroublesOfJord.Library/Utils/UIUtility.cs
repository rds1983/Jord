using Myra;
using Myra.Graphics2D.UI;

namespace TroublesOfJord.Utils
{
	public static class UIUtility
	{
		public static void SetGridStyle(this Grid grid)
		{
			grid.SelectionBackground = DefaultAssets.UITextureRegionAtlas["tree-selection"];
			grid.SelectionHoverBackground = DefaultAssets.UITextureRegionAtlas["button-over"];
		}
	}
}