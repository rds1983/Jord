using Microsoft.Xna.Framework;
using Jord.Core;
using Myra.Graphics2D;
using Jord.Utils;

namespace Jord.UI
{
	public enum MapViewCameraType
	{
		PlayerDisplayPosition,
		PlayerPosition
	}

	public class MapView : MapRender
	{
		public MapViewCameraType CameraType { get; set; }

		protected override void BeforeDraw(RenderContext context)
		{
			base.BeforeDraw(context);

			Map = TJ.Session.Player.Map;

			var pos = TJ.Session.Player.DisplayPosition;
			if (CameraType == MapViewCameraType.PlayerPosition)
			{
				pos = TJ.Session.Player.Position.ToVector();
			}


			var tl = new Vector2(pos.X - GridSize.X / 2, pos.Y - GridSize.Y / 2);

			if (tl.X < 0)
			{
				tl.X = 0;
			}

			if (tl.X >= Map.Width - GridSize.X)
			{
				tl.X = Map.Width - GridSize.X;
			}

			if (tl.Y < 0)
			{
				tl.Y = 0;
			}

			if (tl.Y >= Map.Height - GridSize.Y)
			{
				tl.Y = Map.Height - GridSize.Y;
			}

			TopLeft = tl;
		}

		protected override void BeforeDrawTile(RenderContext context, Tile tile)
		{
			base.BeforeDrawTile(context, tile);

			if (Config.DrawHighlight && tile.Highlighted)
			{
				var screen = GameToScreen(tile.Position);

				var rect = new Rectangle(screen.X, screen.Y, TileSize.X, TileSize.Y);
				context.FillRectangle(rect, Color.LightGreen);
			}
		}
	}
}
