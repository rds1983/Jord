using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace Jord.UI
{
	public class MiniMap: MapNavigationBase
	{
		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			var player = TJ.Session.Player;
			if (player == null)
			{
				return;
			}

			var pos = GameToScreen(player.DisplayPosition);

			DrawAppearance(context, Color.White, new Rectangle(pos.X, pos.Y, TileSize.X, TileSize.Y));
		}
	}
}
