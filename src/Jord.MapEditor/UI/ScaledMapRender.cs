using FontStashSharp;
using Jord.Core;
using Jord.Utils;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using System;
using Microsoft.Xna.Framework;

namespace Jord.MapEditor.UI
{
	internal class ScaledMapRender : Widget
	{
		private Map _map;
		private DateTime? _lastStamp;

		public Map Map
		{
			get
			{
				return _map;
			}

			set
			{
				if (value == _map)
				{
					return;
				}

				_map = value;
				InvalidateMeasure();
			}
		}

		public Vector2 TileSize { get; set; } = new Vector2(32, 32);

		public ScaledMapRender()
		{
			ClipToBounds = true;
			// Background = SpriteBatch.White;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (Map == null)
			{
				return;
			}

			TileSize = new Vector2((float)ActualBounds.Width / Map.Width, (float)ActualBounds.Height / Map.Height);

			var font = TJ.Database.Settings.FontSystem.GetFont(TileSize.Y);
			for (var mapY = 0; mapY < Map.Height; ++mapY)
			{
				for (var mapX = 0; mapX < Map.Width; ++mapX)
				{
					var tile = Map[mapX, mapY];

					var pos = new Vector2(mapX, mapY);
					Vector2 screen = GameToScreen(pos);
					var opacity = 1.0f;
					var appearance = tile.Appearance;

					if (tile.Object != null)
					{
						appearance = tile.Object.Image;
					}

					if (tile.Inventory.Items.Count > 0)
					{
						appearance = tile.Inventory.Items[0].Item.Info.Image;
					}

					if (tile.Creature != null)
					{
						screen = GameToScreen(tile.Creature.DisplayPosition);
						appearance = tile.Creature.Image;
						opacity = tile.Creature.Opacity;
					}

					if (appearance != null)
					{
						var sz = font.MeasureString(appearance.Symbol);
						var screen2 = new Vector2((int)(screen.X + sz.X / 2),
							(int)(screen.Y + sz.Y / 2));
						context.DrawString(font, appearance.Symbol, screen2, appearance.Color * opacity);
					}
				}
			}
		}

		public Vector2 GameToScreen(Vector2 gamePosition)
		{
			var bounds = ActualBounds;
			return new Vector2(bounds.X + (gamePosition.X * TileSize.X),
				bounds.Y + (gamePosition.Y * TileSize.Y));
		}
	}
}
