using FontStashSharp;
using Myra;
using Myra.Graphics2D.UI;
using System.Linq;

namespace Jord.Utils
{
	public static class UIUtility
	{
		private static SpriteFontBase _defaultFont;

		public static void SetGridStyle(this Grid grid)
		{
			grid.SelectionBackground = DefaultAssets.UITextureRegionAtlas["tree-selection"];
			grid.SelectionHoverBackground = DefaultAssets.UITextureRegionAtlas["button-over"];
		}

		public static SpriteFontBase DefaultFont
		{
			get
			{
				if (_defaultFont == null)
				{
					_defaultFont = DefaultAssets.UIStylesheet.Fonts.Values.First();
				}

				return _defaultFont;
			}
		}
	}
}