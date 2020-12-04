using Jord.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;

namespace Jord.UI
{
	public class MapRender : Widget
	{
		private const int SignPeriodInMs = 1000;
		public static readonly Point TileSize = new Point(32, 32);
		private readonly List<Tile> _tilesWithSigns = new List<Tile>();

		private Map _map;
		private DateTime? _lastStamp;

		public SpriteFont Font
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

		private static string NpcMarker(NonPlayer npc, out Color color)
		{
			string result = null;
			color = Color.White;
			switch (npc.Info.CreatureType)
			{
				case CreatureType.Merchant:
					result = "$";
					color = Color.Gold;
					break;
				case CreatureType.Instructor:
					result = "^";
					break;
			}

			return result;
		}

		private void RenderMarker(RenderContext context, Point pos, NonPlayer npc)
		{
			var tileSize = TileSize;

			Color markerColor;
			var marker = NpcMarker(npc, out markerColor);
			if (!string.IsNullOrEmpty(marker))
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
				var measureSize = font.MeasureString(marker);
				var x = screen.X + (tileSize.X - measureSize.X) / 2;
				var y = screen.Y + (tileSize.Y - measureSize.Y) / 2 - 8;

				context.Batch.DrawString(font, marker, new Vector2(x, y), markerColor * opacity);
			}
		}

		private void RenderCreatureDecorations(RenderContext context, Point pos, Creature creature)
		{
			if (creature.Stats.Life.MaximumHP != 0 &&
				creature.Stats.Life.CurrentHP < creature.Stats.Life.MaximumHP)
			{
				var topLeft = GameToScreen(new Vector2(creature.DisplayPosition.X + 0.2f, creature.DisplayPosition.Y + 1.0f));
				var bottomRight = GameToScreen(new Vector2(creature.DisplayPosition.X + 0.8f, creature.DisplayPosition.Y + 1.2f));

				var size = bottomRight - topLeft;

				var hpWidth = creature.Stats.Life.CurrentHP * size.X / creature.Stats.Life.MaximumHP;

				// Red background
				context.FillRectangle(new Rectangle(topLeft.X, topLeft.Y, size.X, size.Y), Color.Red);

				// Green hps
				context.FillRectangle(new Rectangle(topLeft.X, topLeft.Y, (int)hpWidth, size.Y), Color.Green);
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

			if (Map == null)
			{
				return;
			}

			var mapViewPort = new Rectangle((int)TopLeft.X,
				(int)TopLeft.Y,
				gridSize.X + 1,
				gridSize.Y + 1);

			_tilesWithSigns.Clear();
			for (var mapY = mapViewPort.Y; mapY < mapViewPort.Bottom; ++mapY)
			{
				for (var mapX = mapViewPort.X; mapX < mapViewPort.Right; ++mapX)
				{
					if (mapX < 0 || mapX >= Map.Width || mapY < 0 || mapY >= Map.Height)
						continue;

					var pos = new Point(mapX, mapY);
					var tile = Map[pos];

					if (!IgnoreFov && !Map.Light && !tile.IsExplored)
					{
						continue;
					}

					BeforeDrawTile(context, tile);

					var screen = GameToScreen(pos);

					var isInFov = IgnoreFov || tile.IsInFov;
					var opacity = (Map.Light || isInFov) ? 1.0f : 0.5f;
					var appearance = tile.Info.Image;

					if (isInFov)
					{
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
					}

					var rect = new Rectangle(screen.X, screen.Y, tileSize.X, tileSize.Y);
					appearance.Draw(context.Batch, rect, opacity);

					if (!string.IsNullOrEmpty(tile.Sign))
					{
						_tilesWithSigns.Add(tile);
					}

					if (isInFov && tile.Creature != null)
					{
						RenderCreatureDecorations(context, pos, tile.Creature);
					}
				}
			}

			// Draw signs
			foreach (var tile in _tilesWithSigns)
			{
				var screen = GameToScreen(new Vector2(tile.X, tile.Y));
				var sz = Font.MeasureString(tile.Sign);
				context.Batch.DrawString(Font, tile.Sign,
					new Vector2((int)(screen.X + (tileSize.X - sz.X) / 2), (int)(screen.Y + (tileSize.Y - sz.Y) / 2)),
					Color.White);
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