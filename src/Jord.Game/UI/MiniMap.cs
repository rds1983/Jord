using Microsoft.Xna.Framework;
using Myra.Graphics2D;

namespace Jord.UI
{
	public class MiniMap: MapNavigationBase
	{
		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			var pos = GameToScreen(TJ.PlayerDisplayPosition);
			DrawAppearance(context, Color.White, new Rectangle(pos.X, pos.Y, TileSize.X, TileSize.Y));
		}
	}
}
