using TroublesOfJord.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;
using System;

namespace TroublesOfJord.UI
{
	public class MapRender : Widget
	{
		private const int SignPeriodInMs = 1000;
		public static readonly Point TileSize = new Point(32, 32);

		private Map _map;
		private DateTime? _lastStamp;

		public SpriteFont Font
		{
			get; set;
		}

		public SpriteFont SmallFont
		{
			get; set;
		}

		public Vector2 TopLeft
		{
			get; set;
		}

		public Point GridSize
		{
			get; private set;
		}

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

		public bool IgnoreFov;

		public MapRender()
		{
			Font = DefaultAssets.Font;
			SmallFont = DefaultAssets.FontSmall;

			ClipToBounds = true;
			// Background = SpriteBatch.White;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		protected virtual void BeforeDraw(RenderContext context)
		{
		}

		protected virtual void BeforeDrawTile(RenderContext context, Tile tile)
		{
		}

		private void RenderNpc(RenderContext context, Point pos, NonPlayer npc)
		{
			var tileSize = TileSize;

			if (npc.Info.IsMerchant)
			{
				var screen = GameToScreen(new Vector2(pos.X, pos.Y - 0.5f));

				var opacity = 1.0f;

				var now = DateTime.Now;
				if (_lastStamp == null || (now - _lastStamp.Value).TotalMilliseconds >= SignPeriodInMs * 2)
				{
					_lastStamp = now;
				}
				else
				{
					var passed = (now - _lastStamp.Value).TotalMilliseconds;
					if (passed >= SignPeriodInMs * 2)
					{
						_lastStamp = now;
					}
					else if (passed >= SignPeriodInMs)
					{
						opacity = 0.5f + 0.5f * (float)(passed - SignPeriodInMs) / SignPeriodInMs;
					}
					else
					{
						opacity = 1.0f - 0.5f * (float)(passed) / SignPeriodInMs;
					}
				}

				var font = Font;
				var measureSize = font.MeasureString("$");
				var x = screen.X + (tileSize.X - measureSize.X) / 2;
				var y = screen.Y + (tileSize.Y - measureSize.Y) / 2 - 8;

				context.Batch.DrawString(font, "$", new Vector2(x, y), Color.Gold * opacity);
			}

			if (npc.Stats.Life.MaximumHP != 0 &&
				npc.Stats.Life.CurrentHP < npc.Stats.Life.MaximumHP)
			{
				var topLeft = GameToScreen(new Vector2(npc.DisplayPosition.X + 0.2f, npc.DisplayPosition.Y + 1.0f));
				var bottomRight = GameToScreen(new Vector2(npc.DisplayPosition.X + 0.8f, npc.DisplayPosition.Y + 1.2f));

				var size = bottomRight - topLeft;

				var hpWidth = npc.Stats.Life.CurrentHP * size.X / npc.Stats.Life.MaximumHP;

				// Red background
				context.FillRectangle(new Rectangle(topLeft.X, topLeft.Y, size.X, size.Y), Color.Red);

				// Green hps
				context.FillRectangle(new Rectangle(topLeft.X, topLeft.Y, hpWidth, size.Y), Color.Green);
			}
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			var tileSize = TileSize;
			var gridSize = new Point(context.View.Width / tileSize.X,
									 context.View.Height / tileSize.Y);
			GridSize = gridSize;

			BeforeDraw(context);

			if (Font == null || SmallFont == null || Map == null)
			{
				return;
			}

			var mapViewPort = new Rectangle((int)TopLeft.X,
				(int)TopLeft.Y,
				gridSize.X + 1,
				gridSize.Y + 1);

			for (var mapY = mapViewPort.Y; mapY < mapViewPort.Bottom; ++mapY)
			{
				for (var mapX = mapViewPort.X; mapX < mapViewPort.Right; ++mapX)
				{
					if (mapX < 0 || mapX >= Map.Width || mapY < 0 || mapY >= Map.Height)
						continue;

					var pos = new Point(mapX, mapY);
					var tile = Map[pos];

					if (!IgnoreFov && !tile.IsExplored)
					{
						continue;
					}

					BeforeDrawTile(context, tile);

					var screen = GameToScreen(pos);

					var isInFov = IgnoreFov || tile.IsInFov;
					var opacity = isInFov ? 1.0f : 0.5f;
					var appearance = tile.Info.Image;
					if (isInFov && tile.Creature != null)
					{
						screen = GameToScreen(tile.Creature.DisplayPosition);
						appearance = tile.Creature.Image;
						opacity = tile.Creature.Opacity;
					}

					var rect = new Rectangle(screen.X, screen.Y, tileSize.X, tileSize.Y);
					appearance.Draw(context.Batch, rect, opacity);

					var asNpc = tile.Creature as NonPlayer;
					if (isInFov && asNpc != null)
					{
						RenderNpc(context, pos, asNpc);
					}
				}
			}
		}

		public Point GameToScreen(Vector2 gamePosition)
		{
			var bounds = ActualBounds;
			return new Point(bounds.X + (int)((gamePosition.X - TopLeft.X) * TileSize.X),
				bounds.Y + (int)((gamePosition.Y - TopLeft.Y) * TileSize.Y));
		}

		public Point GameToScreen(Point gamePosition)
		{
			var bounds = ActualBounds;
			return new Point(bounds.X + (int)((gamePosition.X - TopLeft.X) * TileSize.X),
				bounds.Y + (int)((gamePosition.Y - TopLeft.Y) * TileSize.Y));
		}

		public Vector2 ScreenToGame(Point position)
		{
			var bounds = ActualBounds;

			position -= bounds.Location;
			var tileSize = TileSize;
			var tilePosition = new Vector2
			{
				X = TopLeft.X + position.X / tileSize.X,
				Y = TopLeft.Y + position.Y / tileSize.Y
			};

			return tilePosition;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (Map == null)
			{
				return Point.Zero;
			}

			var tileSize = TileSize;
			return new Point(Map.Width * tileSize.X,
				Map.Height * tileSize.Y);
		}
	}
}